  // Hàm để lấy dữ liệu khuyến mãi từ API
  async function loadPromotions() {
    try {
        const response = await fetch('https://localhost:7286/api/Controller_Member/getListPromotion');
        if (!response.ok) throw new Error('Không thể lấy dữ liệu khuyến mãi');

        const promotions = await response.json();

        // Lấy 3 khuyến mãi mới nhất
        const latestPromotions = promotions.slice(0, 3); // Giả định rằng các khuyến mãi đã được sắp xếp theo thời gian giảm giá gần nhất

        const promotionList = document.getElementById('promotion-list');

        // Thêm từng khuyến mãi vào danh sách
        latestPromotions.forEach(promotion => {
            const promotionItem = document.createElement('div');
            promotionItem.classList.add('promotion-item');
            promotionItem.innerHTML = `
                <p>🔥 Giảm giá ${promotion.priceOff}% từ ${new Date(promotion.start).toLocaleDateString()} -> ${new Date(promotion.end).toLocaleDateString()}🔥</p>
            `;
            promotionList.appendChild(promotionItem);
        });
    } catch (error) {
        console.error('Lỗi:', error);
        alert('Có lỗi xảy ra khi lấy dữ liệu khuyến mãi.');
    }
}

// Gọi hàm để tải khuyến mãi khi trang được tải
document.addEventListener('DOMContentLoaded', loadPromotions);