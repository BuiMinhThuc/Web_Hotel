let currentPromotionPage = 1; // Biến theo dõi trang hiện tại
const promotionPageSize = 7;  // Số lượng mục mỗi trang

// Hàm gọi API để lấy danh sách khuyến mãi
async function fetchPromotionData(page) {
    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Member/getListPromotionFull?pageSize=${promotionPageSize}&pageNumber=${page}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        updatePromotionTable(data);
    } catch (error) {
        console.error('Lỗi khi lấy dữ liệu khuyến mãi:', error);
        alert('Có lỗi xảy ra khi lấy dữ liệu khuyến mãi');
    }
}

// Hàm cập nhật bảng với dữ liệu khuyến mãi
function updatePromotionTable(promotionData) {
    const tableBody = document.querySelector('#promotionTable tbody');
    tableBody.innerHTML = ''; // Xóa dữ liệu hiện tại trong bảng

    promotionData.forEach(promotion => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${promotion.id}</td>
            <td>${new Date(promotion.start).toLocaleString()}</td>
            <td>${new Date(promotion.end).toLocaleString()}</td>
            <td>${promotion.priceOff}%</td>
            <td>
                <button type="button" class="btn-edit btn-info btn-sm" onclick="showEditPromotionModal(${promotion.id}, '${promotion.start}', '${promotion.end}', ${promotion.priceOff})">Sửa</button>
                <button class="btn-delete btn-danger btn-sm" onclick="deletePromotion(${promotion.id})">Xóa</button>
            </td>
        `;
        tableBody.appendChild(row);
    });

    // Cập nhật số trang hiển thị
    document.getElementById('pageNumPromotion').textContent = currentPromotionPage;
}

// Hàm xử lý khi nhấn nút Trang sau
function nextPromotionPage() {
    currentPromotionPage++;
    fetchPromotionData(currentPromotionPage);
}

// Hàm xử lý khi nhấn nút Trang trước
function prevPromotionPage() {
    if (currentPromotionPage > 1) {
        currentPromotionPage--;
        fetchPromotionData(currentPromotionPage);
    }
}

// Hàm mở modal thêm khuyến mãi
function showAddPromotionModal() {
    document.getElementById('startDate').value = "";
    document.getElementById('endDate').value = "";
    document.getElementById('priceOff').value = "";
    $('#createPromotionModal').modal('show');
}

// Hàm mở modal sửa khuyến mãi
function showEditPromotionModal(id, start, end, priceOff) {
    document.getElementById('promotionId').value = id;
    document.getElementById('editStartDate').value = start;
    document.getElementById('editEndDate').value = end;
    document.getElementById('editPriceOff').value = priceOff;
    $('#editPromotionModal').modal('show');
}

// Hàm hiển thị thông báo đang xử lý
function showLoadingMessage(message) {
    document.getElementById('addPromotion').textContent = message;
    //document.getElementById('loadingMessage').style.display = 'block';
}

// Hàm ẩn thông báo đang xử lý
function hideLoadingMessage() {
    document.getElementById('addPromotion').textContent = "Thêm Khuyến Mãi";
}

// Hàm thêm khuyến mãi
async function addPromotion() {
    const start = document.getElementById('startDate').value;
    const end = document.getElementById('endDate').value;
    const priceOff = document.getElementById('priceOff').value;

    const promotionData = {
        start: start,
        end: end,
        priceOff: parseFloat(priceOff)
    };

    showLoadingMessage('Đang xử lý...');

    try {
        const response = await fetch("https://localhost:7286/api/Controller_Admin/CreatePromotion", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(promotionData)
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        alert("Thêm khuyến mãi thành công");
        $('#createPromotionModal').modal('hide');
        fetchPromotionData(currentPromotionPage); // Tải lại dữ liệu sau khi thêm
    } catch (error) {
        console.error('Lỗi khi thêm khuyến mãi:', error);
        alert('Có lỗi xảy ra khi thêm khuyến mãi');
    } finally {
        hideLoadingMessage();
    }
}

// Hàm cập nhật khuyến mãi
async function updatePromotion() {
    const id = document.getElementById('promotionId').value;
    const start = document.getElementById('editStartDate').value;
    const end = document.getElementById('editEndDate').value;
    const priceOff = document.getElementById('editPriceOff').value;

    const promotionData = {
        id: parseInt(id),
        start: start,
        end: end,
        priceOff: parseFloat(priceOff)
    };

    showLoadingMessage('Đang xử lý...');

    try {
        const response = await fetch("https://localhost:7286/api/Controller_Admin/UpdatePromotion", {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(promotionData)
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        alert("Cập nhật khuyến mãi thành công");
        $('#editPromotionModal').modal('hide');
        fetchPromotionData(currentPromotionPage); // Tải lại dữ liệu sau khi cập nhật
    } catch (error) {
        console.error('Lỗi khi cập nhật khuyến mãi:', error);
        alert('Có lỗi xảy ra khi cập nhật khuyến mãi');
    } finally {
        hideLoadingMessage();
    }
}

// Hàm xóa khuyến mãi
async function deletePromotion(id) {
    if (!confirm('Bạn có chắc chắn muốn xóa khuyến mãi này không?')) {
        return;
    }

    showLoadingMessage('Đang xử lý...');

    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Admin/DeletePromotion?Id=${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        alert('Xóa khuyến mãi thành công');
        fetchPromotionData(currentPromotionPage); // Tải lại dữ liệu sau khi xóa
    } catch (error) {
        console.error('Lỗi khi xóa khuyến mãi:', error);
        alert('Có lỗi xảy ra khi xóa khuyến mãi');
    } finally {
        hideLoadingMessage();
    }
}

// Gắn sự kiện cho các nút
document.getElementById('addPromotion').addEventListener('click', addPromotion);
document.getElementById('updatePromotion').addEventListener('click', updatePromotion);

// Gọi dữ liệu ban đầu khi trang được tải
fetchPromotionData(currentPromotionPage);
