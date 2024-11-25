let currentPageStaff = 1; // Start on the first page
const pageSizeStaff = 7;  // Define the number of items per page

// Function to fetch staff data from the API
async function fetchStaffData(page = currentPageStaff) {
    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Member/getListStaff?pageSize=${pageSizeStaff}&pageNumber=${page}`);
        if (!response.ok) throw new Error('Failed to fetch staff data');
        const data = await response.json();
        updateStaffTable(data);
    } catch (error) {
        console.error('Error fetching staff data:', error);
    }
}

// Function to update the staff table with the fetched data
function updateStaffTable(staffData) {
    const tableBody = document.querySelector('#staffTable tbody');
    tableBody.innerHTML = ''; // Clear existing table data

    staffData.forEach(staff => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${staff.id}</td>
            <td>${staff.name}</td>
            <td>${staff.phone}</td>
            <td>${staff.email}</td>
            <td>${staff.address}</td>
            <td>${staff.gender || 'N/A'}</td>
            <td>
                <button type="button" class="btn btn-info btn-sm" onclick="openEditStaffModal(${staff.id})">Sửa</button>
                <button class="btn btn-danger btn-sm" onclick="deleteStaff(${staff.id})">Xóa</button>
            </td>
        `;
        tableBody.appendChild(row);
    });

    // Update the page number display
    document.getElementById('pageNumStaff').textContent = currentPageStaff;
}

// Pagination handling
function changePage(next = true) {
    currentPageStaff = next ? currentPageStaff + 1 : Math.max(1, currentPageStaff - 1);
    fetchStaffData();
}

document.addEventListener('DOMContentLoaded', () => {
    fetchStaffData(); // Fetch staff data on page load
    document.getElementById('nextBtnStaff').addEventListener('click', () => changePage(true));
    document.getElementById('prevBtnStaff').addEventListener('click', () => changePage(false));
});

// Function to open the edit staff modal
// Hàm mở modal sửa và lấy thông tin từ hàng trong bảng
// Function to open the edit modal and populate data from the table row
// Function to open the edit modal and populate data from the table row
function openEditStaffModal(staffId) {
    // Locate the row containing the staff information using data-id attribute
    const row = document.querySelector(`#staffTable tr[data-id="${staffId}"]`);
    if (!row) {
        alert('Không tìm thấy thông tin nhân viên trong bảng.');
        return;
    }

    // Extract data from the row's cells
    const staffName = row.cells[1].textContent;
    const staffPhone = row.cells[2].textContent;
    const staffEmail = row.cells[3].textContent;
    const staffAddress = row.cells[4].textContent;
    const staffGender = row.cells[5].textContent.trim(); // Lấy giá trị và xóa khoảng trắng

    // Populate modal inputs with extracted data
    document.getElementById("editStaffId").value = staffId;
    document.getElementById("editStaffName").value = staffName;
    document.getElementById("editStaffPhone").value = staffPhone;
    document.getElementById("editStaffEmail").value = staffEmail;
    document.getElementById("editStaffAddress").value = staffAddress;
    document.getElementById("editStaffGender").value = staffGender === "Nam" ? "Nam" : "Nữ"; // Gán giá trị vào dropdown

    // Show the modal
    const editStaffModal = new bootstrap.Modal(document.getElementById('modalEditStaff'));
    editStaffModal.show();
}



function updateStaffTable(staffData) {
    const tableBody = document.querySelector('#staffTable tbody');
    tableBody.innerHTML = ''; // Clear existing table data

    staffData.forEach(staff => {
        const row = document.createElement('tr');
        row.setAttribute('data-id', staff.id); // Add data-id attribute for easy row identification
        row.innerHTML = `
            <td>${staff.id}</td>
            <td>${staff.name}</td>
            <td>${staff.phone}</td>
            <td>${staff.email}</td>
            <td>${staff.address}</td>
            <td>${staff.gender || 'N/A'}</td>
            <td>
                <button type="button" class="btn btn-info btn-sm" onclick="openEditStaffModal(${staff.id})">Sửa</button>
                <button class="btn btn-danger btn-sm" onclick="deleteStaff(${staff.id})">Xóa</button>
            </td>
        `;
        tableBody.appendChild(row);
    });

    // Update the page number display
    document.getElementById('pageNumStaff').textContent = currentPageStaff;
}

// Function to update staff information
async function updateStaff() {
    const staffId = document.getElementById("editStaffId").value;
    const staffName = document.getElementById("editStaffName").value;
    const staffPhone = document.getElementById("editStaffPhone").value;
    const staffAddress = document.getElementById("editStaffAddress").value;
    const staffGender = document.getElementById("editStaffGender").value;
    const staffEmail = document.getElementById("editStaffEmail").value;

    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Admin/UpdateStaff`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                id: staffId,
                name: staffName,
                email: staffEmail,
                phoneNumber: staffPhone,
                address: staffAddress,
                gender: staffGender
            })
        });

        if (!response.ok) throw new Error('Failed to update staff');
        const data = await response.json();
        alert(data.message);
        bootstrap.Modal.getInstance(document.getElementById('modalEditStaff')).hide();
        fetchStaffData(); // Refresh the staff list
    } catch (error) {
        console.error('Error updating staff:', error);
    }
}

// Function to delete staff
async function deleteStaff(staffId) {
    if (!confirm('Bạn có chắc chắn muốn xóa nhân viên này?')) return;

    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Admin/DeleteStaff?Id=${staffId}`, {
            method: "DELETE"
        });

        if (!response.ok) throw new Error('Failed to delete staff');
        const data = await response.json();
        alert(data.message);
        fetchStaffData(); // Refresh the staff list
    } catch (error) {
        console.error('Error deleting staff:', error);
    }
}
document.getElementById('searchInputStaff').addEventListener('keyup', function () {
    const searchTerm = this.value.toLowerCase().trim();
    const rows = document.querySelectorAll('#staffTable tbody tr');

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
