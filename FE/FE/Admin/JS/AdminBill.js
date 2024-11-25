document.addEventListener("DOMContentLoaded", function () {
    const billTableBody = document.querySelector("#billTable tbody");
    const pageSize = 7;
    let currentBillPage = 1;

    // Hàm tải danh sách hóa đơn từ API
    async function loadBills(pageNumber = 1) {
        try {
            const response = await fetch(`https://localhost:7286/api/Controller_Member/getListBill?pageSize=${pageSize}&pageNumber=${pageNumber}`);
            if (!response.ok) throw new Error('Không thể lấy dữ liệu từ API');

            const billData = await response.json();
            displayBills(billData);
            document.getElementById('pageNumBill').innerText = ` ${pageNumber}`;
            currentBillPage = pageNumber;

            // Điều chỉnh trạng thái của các nút phân trang
            document.getElementById('prevBtnBill').disabled = pageNumber === 1;
            document.getElementById('nextBtnBill').disabled = billData.length < pageSize;
        } catch (error) {
            console.error('Lỗi:', error);
            billTableBody.innerHTML = "<tr><td colspan='8'>Có lỗi xảy ra khi tải dữ liệu.</td></tr>";
        }
    }

    // Hàm định dạng ngày giờ theo dạng dd/mm/yyyy hh:mm AM/PM
    function formatDateTime(dateString) {
        const date = new Date(dateString);
        
        // Lấy các phần của ngày giờ
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0'); // tháng từ 0-11
        const year = date.getFullYear();
        
        // Lấy giờ và phút
        let hours = date.getHours();
        const minutes = String(date.getMinutes()).padStart(2, '0');
        
        // Xác định AM hoặc PM
        const ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12 || 12; // chuyển đổi 0 giờ thành 12 AM
        
        // Định dạng lại ngày giờ theo kiểu dd/mm/yyyy hh:mm AM/PM
        return `${day}/${month}/${year} ${hours}:${minutes} ${ampm}`;
    }

    // Hàm hiển thị danh sách hóa đơn lên bảng
    function displayBills(bills) {
        billTableBody.innerHTML = "";
        bills.forEach(bill => {
            const isComplete = bill.payOk === bill.totalPrice;
            const statusContent = isComplete 
                ? `<span>Đã hoàn thành</span>` 
                : `<select class="status-select" data-bill-id="${bill.id}">
                       <option value="Incomplete">Chưa hoàn thành</option>
                       <option value="Complete">Đã hoàn thành</option>
                   </select>`;
    
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${bill.id}</td>
                <td>${bill.bookingId}</td>
                <td>${bill.totalPrice.toLocaleString()} VND</td>
                <td>${formatDateTime(bill.createTime)}</td>
                <td>${bill.payOk.toLocaleString()}/${bill.totalPrice.toLocaleString()} VNĐ</td>
                <td style="width: 50px;">${bill.contentPay}</td>
                <td>${bill.contentPayAll}</td>
                <td>${statusContent}</td>
                <td>
                    <button class="print-btn" data-bill-id="${bill.id}" data-total-price="${bill.totalPrice}" data-pay-ok="${bill.payOk}">
                      <span class="printer-wrapper">
                        <span class="printer-container">
                          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 92 75">
                            <path stroke-width="5" stroke="black"
                              d="M12 37.5H80C85.2467 37.5 89.5 41.7533 89.5 47V69C89.5 70.933 87.933 72.5 86 72.5H6C4.067 72.5 2.5 70.933 2.5 69V47C2.5 41.7533 6.75329 37.5 12 37.5Z"></path>
                            <circle fill="black" r="3" cy="49" cx="78"></circle>
                          </svg>
                        </span>
                        <span class="printer-page-wrapper">
                          <span class="printer-page"></span>
                        </span>
                      </span>
                      In
                    </button>
                </td>
            `;
            billTableBody.appendChild(row);
        });

        function getUserNameFromToken() {
            const token = localStorage.getItem('token');
            
            try {
                // Token gồm 3 phần ngăn cách bởi dấu chấm: Header.Payload.Signature
                const payloadBase64 = token.split('.')[1]; // Lấy phần Payload (phần thứ 2)
                const payload = JSON.parse(atob(payloadBase64)); // Giải mã Base64 và parse thành JSON
                return payload.Id; // Trả về trường "Username"
            } catch (error) {
                console.error('Lỗi khi giải mã token:', error);
                return null;
            }
        }
// Thêm sự kiện cho các phần tử chọn trạng thái
document.querySelectorAll('.status-select').forEach(select => {
    select.addEventListener('change', async function () {
        const billId = this.getAttribute('data-bill-id');
        const newStatus = this.value;

        if (newStatus === "Complete") {
            // Gọi API để cập nhật trạng thái hóa đơn
            try {
                const response = await fetch(`https://localhost:7286/api/Controller_Member/PayCheckOutBill?billId=${billId}&UserId=${getUserNameFromToken()}`, {
                    method: 'PUT',
                });

                // Phân tích phản hồi JSON
                const rs = await response.json();

                // Kiểm tra trạng thái của phản hồi
                if (response.ok && rs.status === 200) {
                    alert(rs.message);
                    // Tải lại danh sách hóa đơn để làm mới dữ liệu
                    loadBills(currentBillPage);
                } else {
                    alert(rs.message || 'Cập nhật trạng thái hóa đơn thất bại.');
                    loadBills(currentBillPage);
                }
            } catch (error) {
                console.error('Lỗi khi gọi API:', error);
                alert("Đã xảy ra lỗi khi kết nối với server.");
            }
        }
    });
});

        // Thêm sự kiện cho các nút "Print"
        document.querySelectorAll('.print-btn').forEach(button => {
            button.addEventListener('click', async function () {
                const totalPrice = parseFloat(button.getAttribute('data-total-price'));
                const payOk = parseFloat(button.getAttribute('data-pay-ok'));
                
                // Kiểm tra số tiền đã thanh toán có bằng tổng số tiền hay không
                if (payOk < totalPrice) {
                    alert("Chưa thể in hóa đơn, vui lòng thanh toán đủ !");
                    return; // Ngăn chặn việc in hóa đơn nếu chưa thanh toán đủ
                }
                try {
                   
                    const billId = button.getAttribute('data-bill-id');
                    const response = await fetch(`https://localhost:7286/api/Controller_Member/PrintBillPdfPDF?billID=${billId}`);
                    if (!response.ok) throw new Error('Không thể tải dữ liệu hóa đơn');
                    
                    const data = await response.json();

                    var docDefinition = {
                        content: [
                            { text: 'FLORENTINO HOTEL', style: 'header', alignment: 'center', color: '#4CAF50', margin: [0, 0, 0, 10] },
                            { text: '92-Bắc Từ Liêm-Hà Nội', alignment: 'center', margin: [0, 0, 0, 5] },
                            { text: 'HÓA ĐƠN THANH TOÁN', style: 'subheader', alignment: 'center', margin: [0, 10, 0, 20], color: '#333' },
                            
                            // Thông tin khách hàng và hóa đơn
                            {
                                columns: [
                                    { text: `Số hóa đơn: ${data.id}`, style: 'customerInfo' },
                                    { text: `Ngày: ${new Date().toLocaleDateString('vi-VN')}`, style: 'customerInfo', alignment: 'right' }
                                ]
                            },
                            { text: `Tên khách hàng: ${data.nameCustomer}`, style: 'customerInfo' },
                            { text: `Số điện thoại: ${data.numberPhone}`, style: 'customerInfo' },
                            { text: `Nhân viên lập hóa đơn: ${data.nameStaff} - Mã nhân viên: ${data.userId}`, style: 'customerInfo' },
                            { text: '---------------------------------------', alignment: 'center', margin: [0, 10, 0, 10] },
                    
                            // Bảng thông tin phòng
                            { text: 'Chi tiết phòng', style: 'tableHeader' },
                            {
                                table: {
                                    widths: ['*', '*', 60, 100],
                                    body: [
                                        [
                                            { text: 'Loại phòng', style: 'tableTitle' },
                                            { text: 'Tên phòng', style: 'tableTitle' },
                                            { text: 'Số lượng', style: 'tableTitle' },
                                            { text: 'Thành tiền (VND)', style: 'tableTitle' }
                                        ],
                                        ...data.listTypeRoom.map(room => {
                                            const roomNames = data.listRoomName
                                                .filter(r => r.roomTypeId === room.roomTypeId)
                                                .map(r => r.name)
                                                .join(", ");
                                            return [
                                                room.roomTypeName,
                                                roomNames,
                                                { text: room.quantity, alignment: 'center' },
                                                { text: (room.priceOfOne * room.quantity).toLocaleString('vi-VN'), alignment: 'right' }
                                            ];
                                        })
                                    ]
                                },
                                layout: {
                                    fillColor: (rowIndex) => rowIndex === 0 ? '#E0E0E0' : null,
                                    hLineColor: (i) => i === 1 ? 'black' : '#E0E0E0',
                                    vLineColor: () => '#E0E0E0'
                                },
                                margin: [0, 0, 0, 10]
                            },
                    
                            // Bảng thông tin dịch vụ
                            { text: 'Chi tiết dịch vụ', style: 'tableHeader' },
                            {
                                table: {
                                    widths: ['*', 60, 100],
                                    body: [
                                        [
                                            { text: 'Dịch vụ', style: 'tableTitle' },
                                            { text: 'Số lượng', style: 'tableTitle' },
                                            { text: 'Thành tiền (VND)', style: 'tableTitle' }
                                        ],
                                        ...data.serviceBill.map(service => [
                                            service.serviceName,
                                            { text: service.quantity, alignment: 'center' },
                                            { text: (service.priceOfOne * service.quantity).toLocaleString('vi-VN'), alignment: 'right' }
                                        ])
                                    ]
                                },
                                layout: {
                                    fillColor: (rowIndex) => rowIndex === 0 ? '#E0E0E0' : null,
                                    hLineColor: (i) => i === 1 ? 'black' : '#E0E0E0',
                                    vLineColor: () => '#E0E0E0'
                                },
                                margin: [0, 0, 0, 10]
                            },
                            
                            // Phụ thu trẻ em (nếu có)
                            ...(data.child > 0 ? [
                                {
                                    text: `Phụ thu trẻ em trên 6 tuổi: ${data.child} x 100,000 VND = ${(data.child * 100000).toLocaleString('vi-VN')} VND`,
                                    style: 'childCharge',
                                    margin: [0, 0, 0, 10]
                                }
                            ] : []),
                    
                            // Thông tin ngày tháng và tổng tiền
                            { text: `Ngày nhận phòng: ${formatDateTime(data.checkInDate)}`, style: 'infoText' },
                            { text: `Ngày trả phòng: ${formatDateTime(data.checkOutDate)}`, style: 'infoText', margin: [0, 0, 0, 10] },
                    
                            {
                                columns: [
                                    { text: 'Tổng tiền:', style: 'totalLabel' },
                                    { text: `${data.totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`, style: 'totalValue', alignment: 'right' }
                                ]
                            },
                            {
                                columns: [
                                    { text: 'Đã thanh toán:', style: 'totalLabel' },
                                    { text: `${data.totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`, style: 'totalValue', alignment: 'right' }
                                ]
                            },
                    
                            { text: '---------------------------------------------', alignment: 'center', margin: [0, 20, 0, 0] },
                            { text: 'FLORENTINO xin cảm ơn quý khách!', alignment: 'center', margin: [0, 5, 0, 0], style: 'thankYou' }
                        ],
                        styles: {
                            header: {
                                fontSize: 24,
                                bold: true
                            },
                            subheader: {
                                fontSize: 16,
                                bold: true
                            },
                            customerInfo: {
                                fontSize: 12,
                                lineHeight: 1.2,
                                margin: [0, 2]
                            },
                            tableHeader: {
                                fontSize: 14,
                                bold: true,
                                margin: [0, 10, 0, 5],
                                color: '#4CAF50'
                            },
                            tableTitle: {
                                bold: true,
                                fillColor: '#EEEEEE',
                                alignment: 'center'
                            },
                            childCharge: {
                                fontSize: 12,
                                italics: true,
                                color: '#FF5722'
                            },
                            infoText: {
                                fontSize: 12,
                                margin: [0, 2]
                            },
                            totalLabel: {
                                fontSize: 12,
                                bold: true,
                                margin: [0, 10, 0, 0]
                            },
                            totalValue: {
                                fontSize: 14,
                                bold: true,
                                color: '#FF5722'
                            },
                            thankYou: {
                                fontSize: 12,
                                italics: true,
                                color: '#4CAF50'
                            }
                        },
                        defaultStyle: {
                            fontSize: 12,
                            lineHeight: 1.4
                        }
                    };
                    
                    
                    
                    
                    
                    
                    // Tạo và tải PDF
                    pdfMake.createPdf(docDefinition).download(`hoadon_khachsan_${data.id}.pdf`);
                } catch (error) {
                    console.error('Lỗi khi gọi API:', error);
                }
            });
        });
    }

    // Chuyển trang trước
    window.prevInvoicePage = function () {
        if (currentBillPage > 1) {
            loadBills(currentBillPage - 1);
        }
    };

    // Chuyển trang sau
    window.nextInvoicePage = function () {
        loadBills(currentBillPage + 1);
    };

    // Khi tab "Quản lý hóa đơn" được mở
    const billTabButton = document.querySelector('[data-tab="bill"]');
    if (billTabButton) {
        billTabButton.addEventListener('click', () => {
            loadBills();
        });
    }

    // Tìm kiếm hóa đơn
    document.getElementById('searchInputBill').addEventListener('keyup', function () {
        const searchTerm = this.value.toLowerCase().trim();
        const rows = document.querySelectorAll('#billTable tbody tr');

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
