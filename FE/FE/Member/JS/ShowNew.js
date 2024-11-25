let currentPageNew11 = 1;
const pageSizenews = 12;

async function loadNews(pageNumber = 1) {
    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Member/GetListNews?pageSize=${pageSizenews}&pageNumber=${pageNumber}`);
        if (!response.ok) throw new Error('Không thể lấy dữ liệu từ API');
        
        const newsData = await response.json();
        displayNews(newsData);

        // Cập nhật số trang hiện tại
        document.getElementById('pageNum').innerText = `Page ${pageNumber}`;
        currentPageNew = pageNumber;

        // Điều chỉnh trạng thái của các nút phân trang
        document.getElementById('prevBtn').disabled = pageNumber === 1;
        document.getElementById('nextBtn').disabled = newsData.length < pageSize;
    } catch (error) {
        console.error('Lỗi:', error);
    }
}

function displayNews(newsList) {
    const newsContainer = document.getElementById('news-list');
    newsContainer.innerHTML = ''; // Xóa nội dung cũ

    newsList.forEach(news => {
        const newsItem = `
            <div class="card mb-3">
                <div class="row g-0">
                    <div class="col-md-4">
                        <img src="${news.urlImg}" class="img-fluid rounded-start" alt="News Image">
                    </div>
                    <div class="col-md-8">
                        <div class="card-body">
                            <h5 class="card-title">${news.title}</h5>
                            <p class="card-text">${news.content}</p>
                        </div>
                    </div>
                </div>
            </div>
        `;
        newsContainer.innerHTML += newsItem;
    });
}

// Chuyển sang trang trước
function prevPage() {
    if (currentPage > 1) {
        loadNews(currentPageNew11 - 1);
    }
}

// Chuyển sang trang kế tiếp
function nextPage() {
    loadNews(currentPageNew11 + 1);
}

// Tải tin tức khi trang được mở
document.addEventListener('DOMContentLoaded', () => loadNews());
