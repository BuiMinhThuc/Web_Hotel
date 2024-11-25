let currentPage = 1;
const pageSize = 7;

// Hàm gọi API để lấy danh sách ca làm việc
function fetchStaffShiftData(pageNumber) {
    fetch(`https://localhost:7286/api/Controller_Member/getListStaffShift?pageSize=${pageSize}&pageNumber=${pageNumber}`)
        .then(response => response.json())
        .then(data => {
            populateTable(data);
        })
        .catch(error => {
            console.error('Error fetching data:', error);
        });
}

// Hàm để điền dữ liệu vào bảng
function populateTable(data) {
    const tableBody = document.querySelector('#staffShiftTable tbody');
    tableBody.innerHTML = ''; // Xóa nội dung cũ

    data.forEach(item => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${item.id}</td>
            <td>${item.userName}</td>
             <td>${item.numberPhone}</td>
            <td>${new Date(item.date).toLocaleDateString()}</td>
            <td>${new Date(item.startTime).toLocaleTimeString()}</td>
            <td>${new Date(item.endTime).toLocaleTimeString()}</td>
            <td>${item.shiftTypes}</td>
        `;
        tableBody.appendChild(row);
    });
}

// Hàm chuyển sang trang tiếp theo
function nextStafShiftPage() {
    currentPage++;
    fetchStaffShiftData(currentPage);
    updatePageNumber();
}

// Hàm chuyển về trang trước
function prevStafShiftPage() {
    if (currentPage > 1) {
        currentPage--;
        fetchStaffShiftData(currentPage);
        updatePageNumber();
    }
}

// Hàm cập nhật số trang hiện tại
function updatePageNumber() {
    document.getElementById('pageNumStafShift').innerText = currentPage;
}

// Gọi API khi trang tải lần đầu
document.addEventListener('DOMContentLoaded', () => {
    fetchStaffShiftData(currentPage);
});

// Tìm kiếm theo ID
document.getElementById('searchInputCustomer').addEventListener('input', function() {
    const searchValue = this.value.toLowerCase();
    const tableRows = document.querySelectorAll('#customerTable tbody tr');

    tableRows.forEach(row => {
        const idCell = row.querySelector('td:first-child');
        if (idCell && idCell.textContent.toLowerCase().includes(searchValue)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
});
