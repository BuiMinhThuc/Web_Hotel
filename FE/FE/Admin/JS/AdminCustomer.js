document.addEventListener("DOMContentLoaded", function () {
    const customerTableBody = document.querySelector("#customerTable tbody");
    const customerHistoryTableBody = document.querySelector("#customerHistoryTable tbody");
    const pageSize = 7; // Số hàng hiển thị trên mỗi trang
    let currentPage = 1;

    // Hàm tải dữ liệu khách hàng từ API với phân trang
    async function loadCustomers(pageNumber = 1) {
        try {
            const response = await fetch(`https://localhost:7286/api/Controller_Admin/getlisShowCustomer?pageSize=${pageSize}&pageNumber=${pageNumber}`);
           
            if (!response.ok) throw new Error('Không thể lấy dữ liệu từ API');
            
            const customerData = await response.json();
            displayCustomers(customerData);

            // Cập nhật số trang hiện tại
            document.getElementById('pageNumCustomer').innerText = ` ${pageNumber}`;
            currentPage = pageNumber;

            // Kiểm tra để bật/tắt các nút phân trang
            document.getElementById('prevBtnCustomer').disabled = pageNumber === 1;
            document.getElementById('nextBtnCustomer').disabled = customerData.length < pageSize;
        } catch (error) {
            console.error('Lỗi:', error);
            customerTableBody.innerHTML = "<tr><td colspan='8'>Có lỗi xảy ra khi tải dữ liệu.</td></tr>";
        }
    }

    async function loadCustomersHistory(pageNumber = 1) {
        try {
            const response = await fetch(`https://localhost:7286/api/Controller_Member/getAllCustomer?pageSize=${pageSize}&pageNumber=${pageNumber}`);
           
            if (!response.ok) throw new Error('Không thể lấy dữ liệu từ API');
            
            const customerData = await response.json();
            displayCustomersHistory(customerData);

            // Cập nhật số trang hiện tại
            document.getElementById('pageNumCustomer').innerText = ` ${pageNumber}`;
            currentPage = pageNumber;

            // Kiểm tra để bật/tắt các nút phân trang
            document.getElementById('prevBtnCustomer').disabled = pageNumber === 1;
            document.getElementById('nextBtnCustomer').disabled = customerData.length < pageSize;
        } catch (error) {
            console.error('Lỗi:', error);
            customerHistoryTableBody.innerHTML = "<tr><td colspan='8'>Có lỗi xảy ra khi tải dữ liệu.</td></tr>";
        }
    }


    // Hiển thị danh sách khách hàng vào bảng
    function displayCustomers(customers) {
        customerTableBody.innerHTML = ""; // Xóa dữ liệu cũ
        customers.forEach(customer => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${customer.id || 'Trống'}</td>
            <td>${customer.fullName || 'Trống'}</td>
            <td>${customer.phone || 'Trống'}</td>
            <td>${customer.email || 'Trống'}</td>
            <td>${customer.cardNumber || 'Trống'}</td>
            <td>${customer.nationality || 'Trống'}</td>
            <td>${customer.gender || 'Trống'}</td>
           
                <td>${new Date(customer.dateOfBirth).toLocaleDateString('vi-VN')|| 'Trống'}</td>
            `;
            customerTableBody.appendChild(row);
        });
    }

    function displayCustomersHistory(customers) {
        customerHistoryTableBody.innerHTML = ""; // Xóa dữ liệu cũ
        customers.forEach(customer => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${customer.id || 'Trống'}</td>
            <td>${customer.fullName || 'Trống'}</td>
            <td>${customer.phone || 'Trống'}</td>
            <td>${customer.email || 'Trống'}</td>
            <td>${customer.cardNumber || 'Trống'}</td>
            <td>${customer.contentPay || 'Trống'}</td>
            <td>${customer.createBooking || 'Trống'}</td>
            <td>${customer.gender || 'Trống'}</td>
           
               
            `;
            customerHistoryTableBody.appendChild(row);
        });
    }






    // Chuyển sang trang kế tiếp
    window.nextCustomerPage = function () {
        loadCustomers(currentPage + 1);
    };

    // Chuyển sang trang trước
    window.prevCustomerPage = function () {
        if (currentPage > 1) {
            loadCustomers(currentPage - 1);
        }
    };

    // Tải danh sách khách hàng khi trang được mở
    loadCustomers(currentPage);



      // Chuyển sang trang kế tiếp
      window.nextCustomerHistoryPage = function () {
        loadCustomersHistory(currentPage + 1);
    };

    // Chuyển sang trang trước
    window.prevCustomerHistoryPage = function () {
        if (currentPage > 1) {
            loadCustomersHistory(currentPage - 1);
        }
    };

    // Tải danh sách khách hàng khi trang được mở
    loadCustomersHistory(currentPage);

    document.getElementById('searchInputCustomer').addEventListener('keyup', function () {
        const searchTerm = this.value.toLowerCase().trim();
        const rows = document.querySelectorAll('#customerTable tbody tr');

        if (!searchTerm) {
            rows.forEach(row => row.style.display = '');
            return;
        }

        const searchTerms = searchTerm.split(/\s+/);

        rows.forEach(row => {
            const rowText = Array.from(row.querySelectorAll('td')).map(cell => cell.innerText.toLowerCase()).join(' ');
            const found = searchTerms.every(term => rowText.includes(term));
            row.style.display = found ? '' : 'none';
        });
    });
});
