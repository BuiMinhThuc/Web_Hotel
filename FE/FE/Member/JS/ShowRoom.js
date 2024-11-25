// Hàm fetch và hiển thị danh sách phòng
async function fetchRooms() {
    try {
        // Fetch room details
        const roomsResponse = await fetch('https://localhost:7286/api/Controller_Member/GetListRoom');
        const rooms = await roomsResponse.json();

        // Fetch room images
        const imagesResponse = await fetch('https://localhost:7286/api/Controller_Member/GetListRoomImage');
        const images = await imagesResponse.json();

        // Map images by room ID
        const imagesMap = {};
        images.forEach(image => {
            if (!imagesMap[image.roomId]) {
                imagesMap[image.roomId] = [];
            }
            imagesMap[image.roomId].push(image.urlImage);
        });

        // Create room cards
        createRoomCards(rooms, imagesMap);
    } catch (error) {
        console.error('Error fetching room data:', error);
    }
}

// Hàm tạo thẻ phòng và thêm vào container
function createRoomCards(rooms, imagesMap) {
    const roomContainer = document.getElementById('roomContainer');
    roomContainer.innerHTML = ''; // Xóa nội dung trước khi thêm mới

    rooms.forEach(room => {
        const imageUrls = imagesMap[room.id] || ['https://via.placeholder.com/300']; // Placeholder if no images
        const roomCard = createRoomCard(room, imageUrls);

        roomContainer.appendChild(roomCard);
    });
}

// Hàm tạo một thẻ phòng
function createRoomCard(room, imageUrls) {
    const roomCard = document.createElement('div');
    roomCard.classList.add('room-card');

    let currentIndex = 0; // Set initial image index

    roomCard.innerHTML = `
        <img src="${imageUrls[currentIndex]}" alt="${room.roomName}" class="room-image">
        <div class="room-info">
            <h5>${room.roomType}: ${room.roomName} <br>         ${room.price} VNĐ/Ngày</h5>
        </div>
        <div class="btn-container">
            <button class="btn-prev">&lt;</button>
            <button class="btn-view">Xem chi tiết</button>
            <button class="btn-next">&gt;</button>
        </div>
    `;

    // Add event listeners
    roomCard.querySelector('.btn-prev').addEventListener('click', () => {
        currentIndex = (currentIndex - 1 + imageUrls.length) % imageUrls.length;
        roomCard.querySelector('.room-image').src = imageUrls[currentIndex];
    });

    roomCard.querySelector('.btn-next').addEventListener('click', () => {
        currentIndex = (currentIndex + 1) % imageUrls.length;
        roomCard.querySelector('.room-image').src = imageUrls[currentIndex];
    });

    roomCard.querySelector('.btn-view').addEventListener('click', () => {
        showRoomDetails(room.roomName);
    });

    return roomCard;
}

// Hàm hiển thị modal chi tiết phòng
// Hàm hiển thị modal chi tiết phòng
async function showRoomDetails(roomName) {
    try {
        console.log(`Fetching details for room: ${roomName}`); // Kiểm tra tên phòng
        const response = await fetch(`https://localhost:7286/api/Controller_Member/getviewRoom?RoomName=${encodeURIComponent(roomName)}`);
        console.log("1")
        if (!response.ok) {
            throw new Error(`Lỗi phản hồi API: ${response.status} ${response.statusText}`);
        }

        const roomDetails = await response.json();
        console.log("Room Details:", roomDetails); // Kiểm tra dữ liệu JSON trả về

        // Kiểm tra nếu dữ liệu rỗng hoặc không đúng định dạng
        if (!roomDetails || !roomDetails.roomName) {
            throw new Error('Dữ liệu phòng không hợp lệ hoặc trống.');
        }

        // Cập nhật thông tin trong modal
        document.getElementById('modalRoomName').textContent = roomDetails.roomName || 'Không có tên phòng';
        document.getElementById('modalRoomType').textContent = roomDetails.roomTypeName || 'Không có loại phòng';
        document.getElementById('modalPrice').textContent = `${(roomDetails.price || 0).toLocaleString('vi-VN')} VNĐ/Ngày`;
        document.getElementById('modalEquipments').innerHTML = roomDetails.equipments && roomDetails.equipments.length > 0
            ? roomDetails.equipments.map(equipment => `<li>${equipment}</li>`).join('')
            : '<li>Không có thiết bị</li>';

        // Hiển thị modal
        const modal = new bootstrap.Modal(document.getElementById('roomDetailModal'));
        modal.show();
    } catch (error) {
        console.error('Error fetching room details:', error);
        alert(`Có lỗi xảy ra khi lấy thông tin phòng: ${error.message}`);
    }
}



// Hàm xử lý sự kiện tìm kiếm
document.getElementById('search-button').addEventListener('click', async () => {
    const checkInDate = document.getElementById('checkin-date').value;
    const checkOutDate = document.getElementById('checkout-date').value;

    if (!checkInDate || !checkOutDate) {
        alert('Vui lòng chọn ngày đến và ngày đi.');
        return;
    }

    try {
        const response = await fetch(`https://localhost:7286/api/Controller_Member/GetListRoomNull?checkIn=${encodeURIComponent(checkInDate)}&checkOut=${encodeURIComponent(checkOutDate)}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Network response was not ok.');
        }

        const rooms = await response.json();
        const imagesResponse = await fetch('https://localhost:7286/api/Controller_Member/GetListRoomImage');
        const images = await imagesResponse.json();

        const imagesMap = {};
        images.forEach(image => {
            if (!imagesMap[image.roomId]) {
                imagesMap[image.roomId] = [];
            }
            imagesMap[image.roomId].push(image.urlImage);
        });

        createRoomCards(rooms, imagesMap);
    } catch (error) {
        console.error('Error:', error);
        alert('Có lỗi xảy ra khi tìm kiếm.');
    }
});

// Hàm lọc phòng theo lựa chọn của người dùng
async function filterRooms() {
    const filterValue = document.getElementById('room-filter').value;
    const roomContainer = document.getElementById('roomContainer');

    roomContainer.innerHTML = ''; // Xóa nội dung cũ

    try {
        let rooms = [];
       
            const response = await fetch(`https://localhost:7286/api/Controller_Member/getRoomNull?RoomName=${filterValue}&pageSize=7&pageNumber=1`);
            rooms = await response.json();
        

        const imagesResponse = await fetch('https://localhost:7286/api/Controller_Member/GetListRoomImage');
        const images = await imagesResponse.json();

        const imagesMap = {};
        images.forEach(image => {
            if (!imagesMap[image.roomId]) {
                imagesMap[image.roomId] = [];
            }
            imagesMap[image.roomId].push(image.urlImage);
        });

        createRoomCards(rooms, imagesMap);
    } catch (error) {
        console.error('Error fetching room data:', error);
        roomContainer.innerHTML = '<p>Có lỗi xảy ra khi lấy dữ liệu phòng.</p>';
    }
}

// Thêm sự kiện cho menu lọc phòng
document.getElementById('room-filter').addEventListener('change', filterRooms);

// Gọi fetchRooms khi trang được tải xong
document.addEventListener('DOMContentLoaded', fetchRooms);


