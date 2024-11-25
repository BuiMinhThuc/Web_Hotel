document.addEventListener("DOMContentLoaded", function () {
    const bookingTableBody = document.querySelector("#bookingTable tbody");
    const searchInput = document.getElementById("searchInputBooking");
    const editModal = new bootstrap.Modal(document.getElementById('editModalBooking'));
    const createModal = new bootstrap.Modal(document.getElementById('bookingModal'));
    const pageSize = 7;
    let currentPage = 1;
    let selectedBookingId = null;

    // Tải danh sách đặt phòng từ API với phân trang
    async function loadBookings(pageNumber = 1) {
        try {
            const response = await fetch(`https://localhost:7286/api/Controller_Member/GetListBooking?pageSize=${pageSize}&pageNumber=${pageNumber}`);
            if (!response.ok) throw new Error('Không thể lấy dữ liệu từ API');
            const bookingData = await response.json();
            displayBookings(bookingData);

            document.getElementById('pageNum').innerText = ` ${pageNumber}`;
            currentPage = pageNumber;
            document.getElementById('prevBtn').disabled = pageNumber === 1;
            document.getElementById('nextBtn').disabled = bookingData.length < pageSize;
        } catch (error) {
            console.error('Lỗi:', error);
            bookingTableBody.innerHTML = "<tr><td colspan='8'>Có lỗi xảy ra khi tải dữ liệu.</td></tr>";
        }
    }

    // Hiển thị danh sách đặt phòng
    function displayBookings(bookings) {
        bookingTableBody.innerHTML = "";
        bookings.forEach(booking => {
            const roomNames = booking.listRoomName.join(", ");
            const roomTypes = booking.listRoomType.map(type => `${type.roomTypeName} (${type.quantity})`).join(", ");

            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${booking.id}</td>
                <td>${booking.billId}</td>
                <td>${booking.customerName}</td>
                <td>${roomTypes}</td>
                <td>${roomNames}</td>
                <td>${new Date(booking.checkInDate).toLocaleDateString('vi-VN')}</td>
                <td>${new Date(booking.checkOutDate).toLocaleDateString('vi-VN')}</td>
                <td>${booking.numberOfChild}</td>
                <td>${booking.numberPhone}</td>
                <td>${booking.cardNumber}</td>
                <td>
                    <button type="button" class="btn-edit btn-info btn-sm" 
                        data-id="${booking.id}" 
                        data-customer-name="${booking.customerName}" 
                        data-card-number="${booking.cardNumber}" 
                        data-phone="${booking.numberPhone}" 
                        data-bs-toggle="modal" data-bs-target="#editModalBooking">Sửa</button>
                    <button class="btn-delete btn-danger btn-sm" data-id="${booking.id}">Xóa</button>
                </td>
            `;
            bookingTableBody.appendChild(row);
        });

        document.querySelectorAll('.btn-edit').forEach(button => button.addEventListener('click', handleEditClick));
        document.querySelectorAll('.btn-delete').forEach(button => button.addEventListener('click', handleDeleteClick));
    }

    // Xử lý khi nhấn "Sửa"
    function handleEditClick() {
        selectedBookingId = this.getAttribute('data-id');
        document.getElementById('customerName').value = this.getAttribute('data-customer-name');
        document.getElementById('customerNumberCard').value = this.getAttribute('data-card-number');
        document.getElementById('customerPhone').value = this.getAttribute('data-phone');
    }

    // Xử lý khi nhấn "Xóa"
    async function handleDeleteClick() {
        const bookingId = this.getAttribute('data-id');
        if (confirm("Bạn có chắc muốn xóa đặt phòng này không?")) {
            await deleteBooking(bookingId);
        }
    }

    // Hàm xóa đặt phòng
    async function deleteBooking(bookingId) {
        try {
            const response = await fetch(`https://localhost:7286/api/Controller_Member/DeleteBooking?request=${bookingId}`, { method: 'DELETE' });
            if (response.ok) {
                loadBookings(currentPage);
            } else {
                const result = await response.json();
                alert(result.message || 'Xóa thất bại, vui lòng thử lại!');
            }
        } catch (error) {
            console.error('Lỗi khi xóa đặt phòng:', error);
            alert('Có lỗi xảy ra khi xóa đặt phòng.');
        }
    }

    // Sự kiện submit form chỉnh sửa
    document.getElementById('updateBookingv1').addEventListener('click', async function (event) {
        event.preventDefault();
        const updatedBooking = {
            id: selectedBookingId,
            customerName: document.getElementById('customerName').value,
            cardNumber: document.getElementById('customerNumberCard').value,
            numberPhone: document.getElementById('customerPhone').value
        };

        try {
            const response = await fetch(`https://localhost:7286/api/Controller_Member/UpdateBooking`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updatedBooking)
            });
            const result = await response.json();

            if (result.status==200) {
                alert(result.message);
                editModal.hide();
                loadBookings(currentPage);
            // } else {
            //     alert( || 'Cập nhật thất bại, vui lòng thử lại!');
            // }
            }}catch (error) {
            console.error('Lỗi cập nhật:', error);
            alert('Có lỗi xảy ra khi cập nhật dữ liệu.');
        }
    });

    // Phân trang
    window.prevPage = function () { if (currentPage > 1) loadBookings(currentPage - 1); };
    window.nextPage = function () { loadBookings(currentPage + 1); };

    // Tìm kiếm
    searchInput.addEventListener('keyup', function () {
        const searchTerm = this.value.toLowerCase();
        document.querySelectorAll('#bookingTable tbody tr').forEach(row => {
            const found = Array.from(row.querySelectorAll('td')).some(cell => cell.innerText.toLowerCase().includes(searchTerm));
            row.style.display = found ? '' : 'none';
        });
    });

    loadBookings();

    // Hàm lấy danh sách loại phòng từ API
    async function fetchRoomTypes() {
        try {
            const response = await fetch('https://localhost:7286/api/Controller_Member/getListRoomTypeName');
            const roomTypes = await response.json();
            const roomListContainer = document.getElementById('roomListContainer');
            roomListContainer.innerHTML = '';

            roomTypes.forEach(room => {
                const roomItem = document.createElement('div');
                roomItem.classList.add('mb-3');
                roomItem.innerHTML = `
                    <label>${room.roomTypename}</label>
                    <div class="input-group">
                        <button type="button" class="btn btn-outline-secondary minus" data-room="${room.roomTypename}">-</button>
                        <input type="number" class="form-control text-center" id="${room.roomTypename}" value="0" min="0" readonly>
                        <button type="button" class="btn btn-outline-secondary plus" data-room="${room.roomTypename}">+</button>
                    </div>
                `;
                roomListContainer.appendChild(roomItem);
            });

            setupRoomControlButtons();
        } catch (error) {
            console.error('Error fetching room types:', error);
        }
    }

    // Thiết lập các nút cộng/trừ
    // Thiết lập các nút cộng/trừ cho loại phòng và số người
    function setupRoomControlButtons() {
        // Thiết lập nút cộng/trừ cho các loại phòng
        document.querySelectorAll('.minus').forEach(btn => btn.addEventListener('click', () => adjustRoomCount(btn, -1)));
        document.querySelectorAll('.plus').forEach(btn => btn.addEventListener('click', () => adjustRoomCount(btn, 1)));
    
        // Thiết lập nút cộng/trừ cho Người lớn
        document.getElementById('adultPlus').addEventListener('click', () => adjustPersonCount('adultCount', 1));
        document.getElementById('adultMinus').addEventListener('click', () => adjustPersonCount('adultCount', -1));
    
        // Thiết lập nút cộng/trừ cho Trẻ em
        document.getElementById('childPlus').addEventListener('click', () => adjustPersonCount('childCount', 1));
        document.getElementById('childMinus').addEventListener('click', () => adjustPersonCount('childCount', -1));
    }
    
    // Hàm điều chỉnh số lượng phòng
    function adjustRoomCount(button, delta) {
        const input = document.getElementById(button.getAttribute('data-room'));
        input.value = Math.max(0, parseInt(input.value) + delta);
    }
    
    // Hàm điều chỉnh số lượng Người lớn và Trẻ em
    function adjustPersonCount(inputId, delta) {
        const input = document.getElementById(inputId);
        input.value = Math.max(0, parseInt(input.value) + delta);
    }

    // Hàm gửi dữ liệu khi nhấn nút "Đặt phòng"
    function setupFormSubmission() {
        const bookingForm = document.getElementById('bookingForm');
        
        const okeee=document.getElementById('ComfirmBooking')
        // bookingForm.addEventListener('submit', async function (event) {
            okeee.addEventListener('click', async function (event) {
            event.preventDefault();

            const bookingData = {
                fullName: document.getElementById('fullName').value.trim(),
                numberPhone: document.getElementById('phone').value.trim(),
                email: document.getElementById('email').value.trim(),
                roomTypeList: Array.from(document.querySelectorAll("#roomListContainer .input-group input")).map(input => ({
                    roomTypeName: input.getAttribute('id'),
                    quantity: parseInt(input.value, 10) || 0
                })),
                checkInDate: document.getElementById('checkInDate').value,
                checkOutDate: document.getElementById('checkOutDate').value,
                childQuantity: parseInt(document.getElementById('childCount').value, 10) || 0,
                cardNumber: document.getElementById('cardNumber2').value
            };

            try {
                const response = await fetch('https://localhost:7286/api/Controller_Member/CreateBookingAdmin', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(bookingData)
                });
                const result = await response.json();

                if (response.ok) {
                    alert(result.message);
                    createModal.hide();
                    loadBookings(currentPage);
                } else {
                    alert(result.message || 'Đặt phòng thất bại, vui lòng thử lại!');
                }
            } catch (error) {
                console.error('Error:', error);
                alert('Đã xảy ra lỗi khi đặt phòng.');
            }
        });
    }

    fetchRoomTypes();
    setupFormSubmission();
});
