document.addEventListener('DOMContentLoaded', function() {
    fetchFeedback(1); // Load initial feedback when page loads
    setupEventListeners();
});

function fetchFeedback(pageNumber) {
    fetch(`https://localhost:7286/api/Controller_Admin/GetListFeedback?pageSize=7&pageNumber=${pageNumber}`)
        .then(response => response.json())
        .then(data => {
            updateReviewList(data);
        })
        .catch(error => console.error('Error fetching feedback data:', error));
}

function updateReviewList(feedbackData) {
    const container = document.querySelector('.review-list');
    container.innerHTML = ''; // Clear existing reviews
    feedbackData.forEach(feedback => {
        const feedbackItem = `
            <div class="review-item">
                <div class="user-photo">
                    <img src="${feedback.urlAvatarCustomer}" alt="User Photo">
                </div>
                <div class="review-content">
                    <strong>${feedback.nameCustomer}</strong>
                    <p>${feedback.content}</p>
                    <div class="rating">${feedback.rate} <span>&#9733;</span></div>
                </div>
            </div>
        `;
        container.innerHTML += feedbackItem;
    });
}

function sendFeedback() {
    const content = document.querySelector('.review-form textarea').value;
    const stars = document.querySelectorAll('.stars span.active').length;
    const userId = getPayloadFromToken().Id;
    console.log(userId)
    if (userId==null) {
        alert('Bạn cần đăng nhập để gửi đánh giá.');
        return;
    }
    console.log(content)
    if(stars.length==0){
        stars.length=5
    }
    const feedbackData = {
        userId: userId,
        content: content,
        rate: stars
    };

    fetch('https://localhost:7286/api/Controller_Admin/CreateFeedback', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(feedbackData)
    })
    .then(response => {
        if (response.status==200) {
            alert('Đánh giá của bạn đã được gửi thành công!');
            fetchFeedback(1); // Reload feedback list
        } else {
            alert('Có lỗi xảy ra, vui lòng thử lại.');
        }
    })
    .catch(error => console.error('Error sending feedback:', error));
}

function getPayloadFromToken() {
    const token = localStorage.getItem('token');
    if (!token) {
        alert('Bạn cần đăng nhập để gửi đánh giá.');
        return;
    }
    try {
        // Token gồm 3 phần ngăn cách bởi dấu chấm: Header.Payload.Signature
        const payloadBase64 = token.split('.')[1]; // Lấy phần Payload (phần thứ 2)
        // Giải mã Base64 và chuyển từ chuỗi không hợp lệ sang chuỗi UTF-8 hợp lệ
        const decodedPayload = decodeURIComponent(escape(atob(payloadBase64)));
        const payload = JSON.parse(decodedPayload); // Giải mã Base64 và parse thành JSON
        return payload; // Trả về
    } catch (error) {
        console.error('Lỗi khi giải mã token:', error);
        return null;
    }
}

function getUserIdFromToken() {
    const token = localStorage.getItem('userToken');
    return token ? parseJwt(token).userId : null;
}

function parseJwt(token) {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
}

function setupEventListeners() {
    document.querySelector('.send-button').addEventListener('click', sendFeedback);

    document.querySelectorAll('.stars span').forEach((star, index) => {
        star.addEventListener('click', () => {
            document.querySelectorAll('.stars span').forEach((s, idx) => {
                s.classList.toggle('active', idx <= index);
            });
        });
    });
}
