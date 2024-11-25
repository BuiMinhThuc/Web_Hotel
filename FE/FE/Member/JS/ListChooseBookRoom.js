
// Toggle dropdown visibility
const dropdownToggle = document.getElementById('dropdownToggle');
const dropdownContainer = document.querySelector('.dropdown-container');
// Sự kiện để hiển thị/ẩn menu dropdown

const dropdownMenu = document.getElementById('dropdownMenu');

dropdownToggle.addEventListener('click', () => {
    dropdownMenu.classList.toggle('active'); // Chuyển đổi lớp active
});

// Hàm cập nhật tóm tắt lựa chọn
function updateSummary() {
    const adultCountInput = document.getElementById('adultCount');
    const childCountInput = document.getElementById('childCount');
    const roomInputs = document.querySelectorAll('#roomListContainer input');
    let totalRooms = 0;

    // Sum up the total number of rooms selected
    roomInputs.forEach(input => {
        totalRooms += parseInt(input.value);
    });

    // Get the current adult and child counts
    let adultCount = parseInt(adultCountInput.value);
    let childCount = parseInt(childCountInput.value);

    // If no rooms are selected, reset adults and children to 0
    if (totalRooms === 0) {
        adultCount = 0;
        childCount = 0;
        adultCountInput.value = 0;
        childCountInput.value = 0;
    } else if (adultCount < totalRooms) {
        // Ensure adults count is at least equal to totalRooms
        adultCountInput.value = totalRooms; // Update the input to match the total rooms
    } else if (adultCount > totalRooms * 2) {
        // Ensure the number of adults does not exceed twice the total rooms
        adultCountInput.value = totalRooms * 2;
        
    }

    // Update the summary text
    document.getElementById('summary').textContent = `${totalRooms} phòng,${adultCount} người lớn ,${childCount} trẻ em `;
}

// Setup event listeners for room control buttons
function setupRoomControlButtons() {
    document.querySelectorAll('.number-control .minus').forEach(btn => {
        btn.addEventListener('click', function () {
            const input = document.getElementById(this.getAttribute('data-type'));
            input.value = Math.max(0, parseInt(input.value) - 1);
            updateSummary();
        });
    });

    document.querySelectorAll('.number-control .plus').forEach(btn => {
        btn.addEventListener('click', function () {
            const input = document.getElementById(this.getAttribute('data-type'));
            input.value = parseInt(input.value) + 1;
            updateSummary();
        });
    });
}

// Increment and Decrement for Adults and Children
document.getElementById('adultPlus').addEventListener('click', function () {
    const adultInput = document.getElementById('adultCount');
    const roomInputs = document.querySelectorAll('#roomListContainer input');
    let totalRooms = 0;

    roomInputs.forEach(input => {
        totalRooms += parseInt(input.value);
    });

    // Only increment adults if there's at least one room selected and not exceeding double the rooms
    if (totalRooms > 0 && adultInput.value < totalRooms * 2) {
        adultInput.value = parseInt(adultInput.value) + 1;
        updateSummary();
    } else if (adultInput.value >= totalRooms * 2) {
       
    }
});

document.getElementById('adultMinus').addEventListener('click', function () {
    const adultInput = document.getElementById('adultCount');
    adultInput.value = Math.max(0, parseInt(adultInput.value) - 1);
    updateSummary();
});

document.getElementById('childPlus').addEventListener('click', function () {
    const childInput = document.getElementById('childCount');
    const roomInputs = document.querySelectorAll('#roomListContainer input');
    let totalRooms = 0;

    roomInputs.forEach(input => {
        totalRooms += parseInt(input.value);
    });

    // Only increment children if there's at least one room selected
    if (totalRooms > 0) {
        childInput.value = parseInt(childInput.value) + 1;
        updateSummary();
    }
});

document.getElementById('childMinus').addEventListener('click', function () {
    const childInput = document.getElementById('childCount');
    childInput.value = Math.max(0, parseInt(childInput.value) - 1);
    updateSummary();
});

// Fetch and display room types
async function fetchAndDisplayRoomTypes() {
    try {
        const response = await fetch('https://localhost:7286/api/Controller_Member/getListRoomTypeName');
        const roomTypes = await response.json();
        const roomListContainer = document.getElementById('roomListContainer');
        roomListContainer.innerHTML = ''; // Clear previous entries

        roomTypes.forEach((room) => {
            const roomContainer = document.createElement('div');
            roomContainer.classList.add('dropdown-item');
            roomContainer.innerHTML = `
                <span>${room.roomTypename}</span>
                <div class="number-control">
                    <button type="button" class="minus" data-type="${room.roomTypename}">-</button>
                    <input type="number" id="${room.roomTypename}" value="0" min="0" disabled>
                    <button type="button" class="plus" data-type="${room.roomTypename}">+</button>
                </div>
            `;
            roomListContainer.appendChild(roomContainer);
        });

        setupRoomControlButtons();
    } catch (error) {
        console.error('Error fetching room types:', error);
    }
}

// Initialize options on page load
document.addEventListener('DOMContentLoaded', fetchAndDisplayRoomTypes);
