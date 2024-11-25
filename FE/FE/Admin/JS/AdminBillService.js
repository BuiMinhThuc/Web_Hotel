document.addEventListener("DOMContentLoaded", function () {
    const billServiceTableBody = document.querySelector("#billServiceTable tbody");
    const pageSize = 7;
    let currentBillServicePage = 1;
    let currentFilter = "All"; // Đổi filter mặc định thành All

    // Hàm tải danh sách dịch vụ đặt từ API
    async function loadBillService(pageNumber = 1, filter = "All") {
        try {
            // Gọi API với filter theo trạng thái
            const response = await fetch(`https://localhost:7286/api/Controller_Member/GetListBillServiceForCase?Type=${filter}&pageSize=${pageSize}&pageNumber=${pageNumber}`);
            if (!response.ok) throw new Error('Không thể lấy dữ liệu từ API');
            
            const billServiceData = await response.json();
            displayBillService(billServiceData);
            document.getElementById('pageNumBillService').innerText = `${pageNumber}`;
            currentBillServicePage = pageNumber;

            // Điều chỉnh trạng thái của các nút phân trang
            document.getElementById('prevBtnBillService').disabled = pageNumber === 1;
            document.getElementById('nextBtnBillService').disabled = billServiceData.length < pageSize;
        } catch (error) {
            console.error('Lỗi:', error);
            billServiceTableBody.innerHTML = "<tr><td colspan='6'>Có lỗi xảy ra khi tải dữ liệu.</td></tr>";
        }
    }

    // Hàm hiển thị dữ liệu lên bảng
    function displayBillService(billServices) {
        billServiceTableBody.innerHTML = "";

        billServices.forEach(service => {
            const row = document.createElement("tr");

            // Kiểm tra trạng thái và hiển thị dropdown nếu chưa hoàn thành
            const statusCell = service.status
                ? `<td>Đã hoàn thành</td>`
                : `<td>
                     <select class="status-dropdown" data-service-id="${service.id}">
                        <option value="false" selected>Chưa hoàn thành</option>
                        <option value="true">Đã hoàn thành</option>
                     </select>
                   </td>`;

            row.innerHTML = `
                <td>${service.id}</td>
                <td>${service.billId}</td>
                <td>${service.roomName}</td>
                <td>${service.serviceName}</td>
                <td>${service.quantity}</td>
                <td>${formatDateTime(service.createTime)}</td>
                ${statusCell}
               <td>
                
                <button class="btn-deletebillservice btn-danger btn-sm" onclick="deleteBillService(${service.id})">Hủy</button>
            </td>
            `;
            billServiceTableBody.appendChild(row);
        });

        // Thêm sự kiện cho dropdown trạng thái
        document.querySelectorAll('.status-dropdown').forEach(dropdown => {
            dropdown.addEventListener('change', async function () {
                const serviceId = dropdown.getAttribute('data-service-id');
                const newStatus = dropdown.value === "true";

                try {
                    // Gọi API để cập nhật trạng thái
                    const response = await fetch(`https://localhost:7286/api/Controller_Member/updateBookingService/${serviceId}`, {
                        method: 'PUT',
                        headers: { 'Content-Type': 'application/json' },
                    });

                    const result = await response.json();

                    if (response.ok) {
                        alert(result.message);

                        // Cập nhật trạng thái trên giao diện
                        const td = dropdown.parentElement;
                        td.innerHTML = 'Đã hoàn thành';
                    } else {
                        alert('Có lỗi xảy ra khi cập nhật trạng thái!');
                    }
                } catch (error) {
                    console.error('Lỗi:', error);
                    alert('Không thể cập nhật trạng thái!');
                }
            });
        });
    }

    // Hàm deleteBillService
    window.deleteBillService = async function (serviceId) {
        try {
            const response = await fetch(`https://localhost:7286/api/Controller_Member/DeleteBillService?service=${serviceId}`, {
                method: 'DELETE'
            });

            const result = await response.json();

            if (response.ok) {
                alert(result.message);
                loadBillService(currentBillServicePage, currentFilter); // Load lại bảng sau khi xóa
            } else {
                alert('Có lỗi xảy ra khi xóa dịch vụ!');
            }
        } catch (error) {
            console.error('Lỗi:', error);
            alert('Không thể xóa dịch vụ!');
        }
    };

    // Hàm định dạng ngày giờ theo kiểu dd/mm/yyyy hh:mm
    function formatDateTime(dateString) {
        const date = new Date(dateString);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        let hours = date.getHours();
        const minutes = String(date.getMinutes()).padStart(2, '0');
        const ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12 || 12;
        return `${day}/${month}/${year} ${hours}:${minutes} ${ampm}`;
    }

    // Hàm khi thay đổi lọc danh sách dịch vụ
    window.getListBillService = function () {
        currentFilter = document.getElementById('billserviceChoose').value || "All"; // Nếu không chọn gì thì mặc định là All
        loadBillService(1, currentFilter); // Tải lại từ trang 1 khi thay đổi lọc
    };

    // Chuyển trang sau
    window.nextBillServicePage = function () {
        loadBillService(currentBillServicePage + 1, currentFilter);
    };

    // Chuyển trang trước
    window.prevBillServicePage = function () {
        if (currentBillServicePage > 1) {
            loadBillService(currentBillServicePage - 1, currentFilter);
        }
    };

    // Khởi tạo tải trang đầu tiên
    loadBillService();
});