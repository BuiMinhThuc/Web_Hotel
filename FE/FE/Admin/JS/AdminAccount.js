let currentAccountPage = 1; // Biến theo dõi trang hiện tại
const accountPageSize = 7;   // Số lượng mục mỗi trang

// Hàm gọi API để lấy danh sách tài khoản
async function fetchAccountData(page) {
    try {
        const response = await fetch(`https://localhost:7286/GetListUser?pageSize=${accountPageSize}&pageNumber=${page}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        updateAccountsTable(data);
    } catch (error) {
        console.error('Error fetching account data:', error);
    }
}

// Hàm cập nhật bảng với dữ liệu tài khoản
function updateAccountsTable(accountsData) {
    const tableBody = document.querySelector('#accountsTable tbody');
    tableBody.innerHTML = ''; // Xóa dữ liệu hiện tại trong bảng

    accountsData.forEach(account => {
        const row = document.createElement('tr');
        row.dataset.id = account.id; // Gắn ID vào thuộc tính data-id của hàng
        row.innerHTML = `
            <td>${account.id}</td>
            <td class="username">${account.userName}</td>
            <td class="fullname">${account.fullName}</td>
            <td class="email">${account.email}</td>
            <td class="phone">${account.numberPhone}</td>
            <td><img src="${account.urlAvatar}" alt="${account.fullName}" style="width: 50px; height: 50px;"></td>
            <td class="address">${account.address}</td>
            <td class="role">${account.roleName}</td>
            <td class="status">${account.isActice ? 'Active' : 'Inactive'}</td>
            <td>
                <button type="button" class="btn-edit btn-info btn-sm" onclick="editAccount(${account.id})">Sửa</button>
                <button class="btn-delete btn-danger btn-sm" onclick="deleteAccount(${account.id})">Xóa</button>
            </td>
        `;
        tableBody.appendChild(row);
    });

    // Gắn sự kiện click cho tất cả các nút "Sửa"


    // Cập nhật số trang hiển thị
    document.getElementById('pageNumAccount').textContent = currentAccountPage;
}


// Hàm mở modal để sửa tài khoản
// Hàm mở modal để sửa tài khoản
// Hàm mở modal để sửa tài khoản
function editAccount(id) {
    // Lấy dòng chứa thông tin tài khoản
    const row = document.querySelector(`#accountsTable tr[data-id='${id}']`);

    // Lấy thông tin từ các cột của hàng
    const username = row.querySelector('.username').textContent;
    const fullname = row.querySelector('.fullname').textContent;
    const email = row.querySelector('.email').textContent;
    console.log("🚀 ~ editAccount ~ email:", email)
    const phone = row.querySelector('.phone').textContent;
    console.log("🚀 ~ editAccount ~ phone:", phone)
    const address = row.querySelector('.address').textContent;
    const role = row.querySelector('.role').textContent;
    const status = row.querySelector('.status').textContent.trim();
    const avatarUrl = row.querySelector('img').getAttribute('src'); // Lấy URL của ảnh đại diện

    // Điền thông tin vào modal
    document.getElementById('userId').value = id;
    document.getElementById('username').value = username;
    document.getElementById('fullname').value = fullname;
    console.log("🚀 ~ editAccount ~ document.getElementById('fullname').value = fullname:", document.getElementById('fullname').value = fullname)
    document.getElementById('email11').value = email;
    document.getElementById('phone11').value = phone;
    document.getElementById('address').value = address;
    document.getElementById('role').value = role;
    document.getElementById('status').value = status === 'Active' ? 'Active' : 'Inactive';

    // Hiển thị ảnh đại diện hiện tại trong modal
    const avatarPreview = document.getElementById('avatarPreview');
    avatarPreview.src = avatarUrl;
    avatarPreview.style.display = 'block'; // Đảm bảo ảnh được hiển thị

    // Hiển thị modal
    $('#editAccountModal').modal('show');
}

// Thay thế ảnh hiện tại khi chọn ảnh mới
document.getElementById('avatar11').addEventListener('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            // Cập nhật src của thẻ img để hiển thị ảnh mới
            document.getElementById('avatarPreview').src = e.target.result;
        };
        reader.readAsDataURL(file);
    }
});
// Hàm xử lý khi nhấn nút Cập nhật
document.getElementById('updateUser').addEventListener('click', async function () {
    const userId = document.getElementById('userId').value;
    const username = document.getElementById('username').value;
    const fullname = document.getElementById('fullname').value;
    console.log("🚀 ~ fullname:", fullname)
    const email = document.getElementById('email11').value;
    console.log("🚀 ~ email:", email)
    const phone = document.getElementById('phone11').value;
    const address = document.getElementById('address').value;
    const role = document.getElementById('role').value;
    const status = document.getElementById('status').value;
    const avatar = document.getElementById('avatar11').files[0];
    console.log("🚀 ~ avatar:", avatar)

    let formData = new FormData();
    formData.append('id', userId);
    formData.append('UserName', username);
    formData.append('FullName', fullname);
    formData.append('Email', email);
    formData.append('NumberPhone', phone);
    formData.append('Address', address);
    formData.append('RoleName', role);
    formData.append('IsActice', status === 'Active');
    if (avatar) {
        formData.append('UrlAvatar', avatar);
    }

    // Hiển thị trạng thái loading
    const updateButton = document.getElementById('updateUser');
    updateButton.disabled = true;
    updateButton.textContent = 'Đang cập nhật...';

    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Admin/UpdateUserf`, {
            method: 'PUT',
            body: formData,
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        alert('Cập nhật tài khoản thành công');
        $('#editAccountModal').modal('hide');
        fetchAccountData(currentAccountPage); // Tải lại dữ liệu bảng
    } catch (error) {
        console.error('Error updating user:', error);
        alert('Có lỗi xảy ra khi cập nhật');
    } finally {
        // Ẩn trạng thái loading và bật lại nút
        updateButton.disabled = false;
        updateButton.textContent = 'Cập Nhật';
    }
});

// Hàm xóa tài khoản
async function deleteAccount(id) {
    const confirmDelete = confirm('Bạn có chắc chắn muốn xóa tài khoản này không?');
    if (confirmDelete) {
        try {
            const response = await fetch(`https://localhost:7286/api/Controller_Admin/DeleteUser?Id=${id}`, { method: 'DELETE' });
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            alert('Xóa tài khoản thành công');
            fetchAccountData(currentAccountPage); // Lấy lại dữ liệu sau khi xóa
        } catch (error) {
            console.error('Error deleting account:', error);
            alert('Có lỗi xảy ra khi xóa tài khoản');
        }
    }
}

// Hàm xử lý khi nhấn nút Trang sau
function nextAccountPage() {
    currentAccountPage++;
    fetchAccountData(currentAccountPage);
}

// Hàm xử lý khi nhấn nút Trang trước
function prevAccountPage() {
    if (currentAccountPage > 1) {
        currentAccountPage--;
        fetchAccountData(currentAccountPage);
    }
}

// Gọi dữ liệu ban đầu khi trang được tải
fetchAccountData(currentAccountPage);


document.getElementById('searchInputAccount').addEventListener('input', function() {
    const searchValue = this.value.toLowerCase();
    const table = document.getElementById('accountsTable');
    const rows = table.getElementsByTagName('tbody')[0].getElementsByTagName('tr');

    for (let row of rows) {
        const idCell = row.getElementsByTagName('td')[0]; // Assuming ID is the first column
        if (idCell) {
            const idText = idCell.textContent || idCell.innerText;
            row.style.display = idText.toLowerCase().includes(searchValue) ? '' : 'none';
        }
    }
});

