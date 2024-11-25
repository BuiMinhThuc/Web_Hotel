let currentNewsPage = 1; // Biến theo dõi trang hiện tại
const newsPageSize = 7;  // Số lượng mục mỗi trang
let editingNewsId = null; // Biến theo dõi ID của tin tức đang chỉnh sửa

// Hàm gọi API để lấy danh sách tin tức
async function fetchNewsData(page) {
    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Member/GetListNews?pageSize=${newsPageSize}&pageNumber=${page}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        updateNewsTable(data);
    } catch (error) {
        console.error('Lỗi khi lấy dữ liệu tin tức:', error);
    }
}

// Hàm cập nhật bảng tin tức với dữ liệu lấy được
function updateNewsTable(newsData) {
    const tableBody = document.querySelector('#newsTable tbody');
    tableBody.innerHTML = ''; // Xóa dữ liệu hiện tại trong bảng

    newsData.forEach(news => {
        const row = document.createElement('tr');
        row.dataset.id = news.id; // Gắn ID vào thuộc tính data-id của hàng
        row.innerHTML = `
            <td>${news.id}</td>
            <td class="title">${news.title}</td>
            <td class="content">${news.content}</td>
            <td><img src="${news.urlImg}" alt="${news.title}" style="width: 50px; height: 50px;"></td>
            <td>
                <button type="button" class="btn-edit btn-info btn-sm" onclick="editNews(${news.id})">Sửa</button>
                <button class="btn-delete btn-danger btn-sm" onclick="deleteNews(${news.id})">Xóa</button>
            </td>
        `;
        tableBody.appendChild(row);
    });

    // Cập nhật số trang hiện tại
    document.getElementById('pageNumNews').textContent = currentNewsPage;
}

// Hàm mở modal để thêm tin tức
function showAddNewsModal() {
    editingNewsId = null; // Đặt ID chỉnh sửa về null (thêm mới)
    document.getElementById("title").value = "";
    document.getElementById("content").value = "";
    document.getElementById("imgNews").value = "";
    document.getElementById("avatarNewsPreview").style.display = "none";
   
    if(editingNewsId!=null){
        $('#editNewsModal').modal('show'); // Hiển thị modal sửa
    }else{
        $('#createNewsModal').modal('show'); // Hiển thị modal thêm
    }
}

// Hàm mở modal để sửa tin tức
function editNews(id) {
    // Tìm dòng chứa thông tin tin tức
    const row = document.querySelector(`#newsTable tr[data-id='${id}']`);

    // Lấy thông tin từ các cột của hàng
    const title = row.querySelector('.title').textContent;
    const content = row.querySelector('.content').textContent;
    const imageUrl = row.querySelector('img').getAttribute('src');

    // Điền thông tin vào modal
    editingNewsId = id;
    document.getElementById('titleEdit').value = title;
    document.getElementById('contentEdit').value = content;
    document.getElementById('avatarNewsPreviewEdit').src = imageUrl;
    document.getElementById('avatarNewsPreviewEdit').style.display = 'block';

    // Hiển thị modal sửa
    $('#editNewsModal').modal('show');
}

// Hàm xử lý thay đổi ảnh để cập nhật preview
document.getElementById('imgNews').addEventListener('change', function(event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function(e) {
            document.getElementById('avatarNewsPreview').src = e.target.result;
            document.getElementById('avatarNewsPreview').style.display = 'block';
        };
        reader.readAsDataURL(file);
    }
});

document.getElementById('imgNewsEdit').addEventListener('change', function(event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function(e) {
            document.getElementById('avatarNewsPreviewEdit').src = e.target.result;
            document.getElementById('avatarNewsPreviewEdit').style.display = 'block';
        };
        reader.readAsDataURL(file);
    }
});



// Hàm lưu hoặc cập nhật tin tức
async function saveNews() {
    const title = document.getElementById('title').value.trim();
    const content = document.getElementById('content').value.trim();
    const imgNews = document.getElementById('imgNews').files[0];


    const titleEdit = document.getElementById('titleEdit').value.trim();
    const contentEdit = document.getElementById('contentEdit').value.trim();
    const imgNewsEdit = document.getElementById('imgNewsEdit').files[0];

if(editingNewsId==null)

    if (!title || !content || (editingNewsId === null && !imgNews)) {
        alert("Vui lòng nhập đủ thông tin");
        return;
    }
   
    const formData = new FormData();
    if(editingNewsId!=null){
        formData.append("Id", editingNewsId);
        formData.append("Title", titleEdit);
        formData.append("Content", contentEdit);
       
    }

    formData.append("Title", title);
    formData.append("Content", content);
    if (imgNews) {
        formData.append("UrlImg", imgNews);
    }
    if(imgNewsEdit){
        formData.append("UrlImg", imgNewsEdit)
    }
    

    const apiUrl = editingNewsId === null ? 
        "https://localhost:7286/api/Controller_Admin/CreateNews" : 
        `https://localhost:7286/api/Controller_Admin/UpdateNews`;

    const method = editingNewsId === null ? "POST" : "PUT";

    try {
        const response = await fetch(apiUrl, {
            method: method,
            body: formData
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        alert(editingNewsId === null ? "Thêm tin tức thành công" : "Cập nhật tin tức thành công");
        $('#createNewsModal').modal('hide');
        $('#editNewsModal').modal('hide');
        fetchNewsData(currentNewsPage); // Tải lại danh sách tin tức
    } catch (error) {
        console.error('Lỗi khi lưu tin tức:', error);
        alert('Có lỗi xảy ra khi lưu tin tức');
    }
}

// Hàm xóa tin tức
async function deleteNews(id) {
    if (!confirm('Bạn có chắc chắn muốn xóa tin tức này không?')) {
        return;
    }

    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Admin/DeleteNews?Id=${id}`, { method: 'DELETE' });
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        alert('Xóa tin tức thành công');
        fetchNewsData(currentNewsPage); // Tải lại dữ liệu sau khi xóa
    } catch (error) {
        console.error('Lỗi khi xóa tin tức:', error);
        alert('Có lỗi xảy ra khi xóa tin tức');
    }
}

// Hàm xử lý chuyển trang
function nextNewsPage() {
    currentNewsPage++;
    fetchNewsData(currentNewsPage);
}

function prevNewsPage() {
    if (currentNewsPage > 1) {
        currentNewsPage--;
        fetchNewsData(currentNewsPage);
    }
}

// Gắn sự kiện cho nút "Thêm tin tức" và "Cập Nhật"
document.getElementById('addNews').addEventListener('click', saveNews);
document.getElementById('updateNews').addEventListener('click', saveNews);

// Tải dữ liệu ban đầu khi trang được tải
fetchNewsData(currentNewsPage);
