document.addEventListener("DOMContentLoaded", () => {
    let currentPage = 1;
    const pageSize = 7; // Number of rooms displayed per page
    
    // Initial data loading
    loadRoomTypes();
    loadRoomData();

    // API URL to fetch room data with pagination
    function getApiURL(roomName) {
        const encodedRoomName = encodeURIComponent(roomName);
        return `https://localhost:7286/api/Controller_Member/getRoomforCase?RoomName=${encodedRoomName}&pageSize=${pageSize}&pageNumber=${currentPage}`;
    }

    // Function to load room data
    function loadRoomData(roomName = 'all') {
        fetch(getApiURL(roomName))
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error fetching data from API");
                }
                return response.json();
            })
            .then(data => {
                renderRoomTable(data);
                updatePaginationButtons(data.length);
            })
            .catch(error => {
                console.error("Error:", error);
            });
    }

    // Function to render room data in the table
    function renderRoomTable(data) {
        const roomTableBody = document.querySelector("#roomTable tbody");
        roomTableBody.innerHTML = "";

        data.forEach(room => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${room.id}</td>
                <td>${room.roomName}</td>
                <td>${room.roomType}</td>
                <td>${room.price.toLocaleString('vi-VN')} VND</td>
                <td>
                    <button type="button" class="btn btn-info btn-sm" onclick="openEditRoomModal(${room.id}, '${room.roomName}', '${room.roomType}')">Sửa</button>
                    <button class="btn btn-danger btn-sm" onclick="deleteRoom(${room.id})">Xóa</button>
                    <button class="btn btn-success btn-sm" onclick="openManageRoomImagesModal(${room.id})">Ảnh</button>
                    <button class="btn btn-equiment btn-success btn-sm" onclick="openEquipmentModal(${room.id})" style="background-color:#6c52de;">Thiết bị</button>
                    
                </td>
            `;
            roomTableBody.appendChild(row);
        });

        document.getElementById("pageNumRoom").innerText = currentPage;
    }

    // Function to update pagination buttons
    function updatePaginationButtons(dataLength) {
        document.getElementById("nextBtnRoom").disabled = dataLength < pageSize;
        document.getElementById("prevBtnRoom").disabled = currentPage === 1;
    }

    // Event listeners for pagination buttons
    document.getElementById("nextBtnRoom").addEventListener("click", () => {
        currentPage++;
        loadRoomData(document.getElementById('room-filter').value);
    });

    document.getElementById("prevBtnRoom").addEventListener("click", () => {
        if (currentPage > 1) {
            currentPage--;
            loadRoomData(document.getElementById('room-filter').value);
        }
    });

    // Load room types and populate dropdown
    function loadRoomTypes() {
        fetch('https://localhost:7286/api/Controller_Member/getListRoomTypeName')
            .then(response => response.json())
            .then(data => {
                populateRoomTypeDropdown(data);
            })
            .catch(error => console.error('Error fetching room types:', error));
    }

    function populateRoomTypeDropdown(roomTypes) {
        const roomTypeDropdown = document.getElementById('room-filter');
        roomTypeDropdown.innerHTML = '<option value="all">Tất cả các phòng</option>'; // Add default option

        roomTypes.forEach(roomType => {
            const option = document.createElement('option');
            option.value = roomType.roomTypename;
            option.textContent = roomType.roomTypename;
            roomTypeDropdown.appendChild(option);
        });
    }

    // Search handling
    function toggleSearchUI(showSearch) {
        const search = document.getElementById('search');
        const destroy = document.getElementById('Detroy');
        const tabSearch = document.getElementById('TabSearch');
        if (showSearch) {
            search.style.display = 'none';
            tabSearch.style.display = 'flex';
            destroy.style.display = 'block';
        } else {
            search.style.display = 'block';
            tabSearch.style.display = 'none';
            destroy.style.display = 'none';
        }
    }

    function searchRooms() {
        const checkin = document.getElementById('checkin-date').value;
        const checkout = document.getElementById('checkout-date').value;
        const roomType = document.getElementById('room-filter').value;

        if (!checkin || !checkout) {
            alert('Vui lòng chọn ngày đến và ngày đi');
            return;
        }

        const apiURL = `https://localhost:7286/api/Controller_Member/getRoomNullFull?RoomName=${encodeURIComponent(roomType)}&CheckIn=${encodeURIComponent(checkin)}&CheckOut=${encodeURIComponent(checkout)}&pageSize=${pageSize}&pageNumber=${currentPage}`;

        fetch(apiURL)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error fetching room availability.");
                }
                return response.json();
            })
            .then(data => {
                renderRoomTable(data);
                updatePaginationButtons(data.length);
            })
            .catch(error => {
                console.error("Error:", error);
            });
    }

    // Event listeners for search and reset
    document.getElementById('search').addEventListener('click', () => toggleSearchUI(true));
    document.getElementById('Detroy').addEventListener('click', () => {
        toggleSearchUI(false);
        loadRoomData();
    });
    document.getElementById('SearchReal').addEventListener('click', searchRooms);

    // Room filter change event
    document.getElementById('room-filter').addEventListener('change', (event) => {
        currentPage = 1;
        loadRoomData(event.target.value);
    });
});

// Modal handling functions (Add/Edit)
function openAddRoomModal() {
    const addRoomModal = new bootstrap.Modal(document.getElementById('modalAddRoom'));
    loadRoomTypes('addRoomType'); // Load room types for the add room modal
    addRoomModal.show();
}
// Function to load room types into a select element
function loadRoomTypes(selectId, selectedType = null) {
    fetch("https://localhost:7286/api/Controller_Member/getListRoomTypeName")
        .then(response => response.json())
        .then(data => {
            const select = document.getElementById(selectId);
            select.innerHTML = ""; // Clear existing options
            data.forEach(type => {
                const option = document.createElement("option");
                option.value = type.roomTypename;
                option.text = type.roomTypename;
                if (type.roomTypename === selectedType) {
                    option.selected = true;
                }
                select.appendChild(option);
            });
        })
        .catch(error => console.error('Error loading room types:', error));
}
function openEditRoomModal(roomId, roomName, roomType) {
    document.getElementById("editRoomId").value = roomId;
    document.getElementById("editRoomName").value = roomName;
    loadRoomTypes('editRoomType', roomType); // Load room types for the edit room modal
    const editRoomModal = new bootstrap.Modal(document.getElementById('modalEditRoom'));
    editRoomModal.show();
}

// Adding, updating, and deleting room functions
function addRoom() {
    const roomName = document.getElementById("addRoomName").value;
    const roomType = document.getElementById("addRoomType").value;

    fetch("https://localhost:7286/api/Controller_Admin/CreateRoom", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ roomName, roomTypeName: roomType })
    })
    .then(response => response.json())
    .then(data => {
        alert(data.message);
        bootstrap.Modal.getInstance(document.getElementById('modalAddRoom')).hide();
        loadRoomData(document.getElementById('room-filter').value)// Refresh room list
    })
    .catch(error => console.error('Error adding room:', error));
}

function updateRoom() {
    const roomId = document.getElementById("editRoomId").value;
    const roomName = document.getElementById("editRoomName").value;
    const roomType = document.getElementById("editRoomType").value;

    fetch("https://localhost:7286/api/Controller_Admin/UpdateRoom", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: roomId, roomName, roomTypeName: roomType })
    })
    .then(response => response.json())
    .then(data => {
        alert(data.message);
        bootstrap.Modal.getInstance(document.getElementById('modalEditRoom')).hide();
        loadRoomData(document.getElementById('room-filter').value) // Refresh room list
    })
    .catch(error => console.error('Error updating room:', error));
}

function deleteRoom(roomId) {
    if (!confirm('Bạn có chắc chắn muốn xóa phòng này?')) return;

    fetch(`https://localhost:7286/api/Controller_Admin/DeleteRoom?request=${roomId}`, {
        method: "DELETE"
    })
    .then(response => response.json())
    .then(data => {
        alert(data.message);
        loadRoomData(document.getElementById('room-filter').value) // Refresh room list
    })
    .catch(error => console.error('Error deleting room:', error));
}



let roomIdMax=1
// Function to open modal and load equipment by Room ID
function openEquipmentModal(roomId) {

    
    roomIdMax=roomId
    fetch(`https://localhost:7286/api/Controller_Admin/GetEquipmentbyRoomId?RoomId=${roomId}`)
      .then(response => response.json())
      .then(data => {
        const equipmentList = document.getElementById('equipmentList');
        equipmentList.innerHTML = ''; // Clear previous content
  
        data.forEach(equipment => {
          const row = document.createElement('tr');
          row.innerHTML = `
            <td>${equipment.name}</td>
            <td>
              <button class="btn btn-warning" onclick="editEquipment(${equipment.id}, '${equipment.name}')">Sửa</button>
              <button class="btn btn-danger" onclick="deleteEquipment(${equipment.id})">Xóa</button>
            </td>
          `;
          equipmentList.appendChild(row);
        });
        
        // Show the modal
        const modal = new bootstrap.Modal(document.getElementById('equipmentModal'));
        modal.show();
      });
  }
  
  // Function to add a new equipment
  function addEquipment() {
    const roomId = roomIdMax; // Replace with actual room ID if dynamic
    const nameEquipment = document.getElementById('newEquipmentName').value;
  
    fetch('https://localhost:7286/api/Controller_Admin/CreateEquipment', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ roomId, nameEquipment })
    })
      .then(response => response.json())
      .then(data => {
        if (data.status === 200) {
          alert(data.message);
          openEquipmentModal(roomId); // Refresh the list
        }
      });
  }
  
  // Function to edit an existing equipment
  function editEquipment(id, currentName) {
    const newName = prompt("Nhập tên mới cho thiết bị:", currentName);
    if (newName) {
      fetch('https://localhost:7286/api/Controller_Admin/UpdateEquipment', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id, roomId: roomIdMax, nameEquipment: newName })
      })
        .then(response => response.json())
        .then(data => {
          if (data.status === 200) {
            alert(data.message);
            openEquipmentModal(roomIdMax); // Refresh the list
          }
        });
    }
  }
  
  // Function to delete an equipment
  function deleteEquipment(id) {
    if (confirm("Bạn có chắc chắn muốn xóa thiết bị này?")) {
      fetch(`https://localhost:7286/api/Controller_Admin/DeleteEquipment?Id=${id}`, {
        method: 'DELETE'
      })
        .then(response => response.json())
        .then(data => {
          if (data.status === 200) {
            alert(data.message);
            openEquipmentModal(roomIdMax); // Refresh the list
          }
        });
    }
  }
  