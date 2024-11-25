let totalSales = 0;
let totalOrders = 0;

// Hàm gọi API để lấy dữ liệu hóa đơn
async function fetchBillData(pageSize = 100, pageNumber = 1) {
    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Member/getListBill?pageSize=${pageSize}&pageNumber=${pageNumber}`);
        const data = await response.json();
        processBillData(data);
    } catch (error) {
        console.error('Error fetching bill data:', error);
    }
}

// Hàm xử lý dữ liệu hóa đơn
function processBillData(billData) {
    totalSales = 0;
    totalOrders = billData.length;

    const salesData = {};

    billData.forEach(item => {
        // Tính tổng doanh thu
        totalSales += item.payOk;

        // Tính toán doanh thu theo ngày
        const date = new Date(item.createTime).toLocaleDateString();
        salesData[date] = (salesData[date] || 0) + item.payOk;
    });

    // Cập nhật giá trị lên giao diện
    document.getElementById('totalSalesValue').innerText = totalSales.toLocaleString() + ' VNĐ';
    document.getElementById('totalOrdersValue').innerText = totalOrders;

    // Vẽ biểu đồ doanh thu
    renderSalesChart(salesData);
}

// Hàm vẽ biểu đồ doanh thu
function renderSalesChart(salesData) {
    const ctx = document.getElementById('totalSalesChart').getContext('2d');
    const labels = Object.keys(salesData);
    const data = Object.values(salesData);

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Doanh thu (VNĐ)',
                data: data,
                backgroundColor: 'rgba(75, 192, 192, 0.6)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

// Hàm gọi API để lấy loại phòng hot
async function fetchRoomTypeHot() {
    try {
        const response = await fetch('https://localhost:7286/api/Controller_Member/getRoomTypeHot');
        const data = await response.json();
        document.getElementById('TypeRoomName').innerText = data.typeRoomHot;
        document.getElementById('TotalQuantity').innerText = `Số lượng: ${data.quantity}`;
    } catch (error) {
        console.error('Error fetching room type hot:', error);
    }
}

// Gọi API khi tải trang
document.addEventListener('DOMContentLoaded', () => {
    fetchBillData();
    fetchRoomTypeHot();
});
document.addEventListener("DOMContentLoaded", function() {
    // Gọi API lấy số người lớn
    fetch('https://localhost:7286/api/Controller_Admin/GetAdultinHotel')
        .then(response => response.json())
        .then(data => {
            document.getElementById("totalAdult").innerText = data || 0; // Giả sử API trả về đối tượng có trường count
        })
        .catch(error => console.error('Lỗi khi lấy số người lớn:', error));

    // Gọi API lấy số trẻ em
    fetch('https://localhost:7286/api/Controller_Admin/GetChildinHotel')
        .then(response => response.json())
        .then(data => {
            document.getElementById("totalChild").innerText = data || 0; // Giả sử API trả về đối tượng có trường count
        })
        .catch(error => console.error('Lỗi khi lấy số trẻ em:', error));

    fetch('https://localhost:7286/api/Controller_Admin/GetNumTypeRoonAloneActiveHotel')
        .then(response => response.json())
        .then(data => {
            document.getElementById("TypeAlone").innerText = data || 0; 
        })
        .catch(error => console.error('Lỗi khi lấy số người lớn:', error));
    fetch('https://localhost:7286/api/Controller_Admin/GetNumTypeRoonDoubleActiveHotel')
        .then(response => response.json())
        .then(data => {
            document.getElementById("TypeDouble").innerText = data || 0; 
        })
        .catch(error => console.error('Lỗi khi lấy số người lớn:', error));
    fetch('https://localhost:7286/api/Controller_Admin/GetNumTypeRoonVipActiveHotel')
        .then(response => response.json())
        .then(data => {
            document.getElementById("TypeVip").innerText = data || 0; 
        })
        .catch(error => console.error('Lỗi khi lấy số người lớn:', error));


});
