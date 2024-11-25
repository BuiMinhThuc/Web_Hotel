// JavaScript for CRUD operations on RoomType Table with Pagination Support

document.addEventListener("DOMContentLoaded", function() {
    loadRoomTypeData(1);

    // Add Room Type Event
    document.querySelector("#createRoomTypeModal .btn-primary").addEventListener("click", function() {
        const roomTypeName = document.querySelector("#createRoomTypeModal input[name='roomTypeName']").value;
        const dailyPrice = document.querySelector("#createRoomTypeModal input[name='dailyPrice']").value;
        const adults = document.querySelector("#createRoomTypeModal input[name='adults']").value;
        createRoomType(roomTypeName, dailyPrice, adults);
    });

    // Edit Room Type Event
    document.querySelector("#editRoomTypeModal .btn-primary").addEventListener("click", function() {
        const id = document.querySelector("#editRoomTypeModal input[name='id']").value;
        const roomTypeName = document.querySelector("#editRoomTypeModal input[name='roomTypeName']").value;
        const dailyPrice = document.querySelector("#editRoomTypeModal input[name='dailyPrice']").value;
        const adults = document.querySelector("#editRoomTypeModal input[name='adults']").value;
        updateRoomType(id, roomTypeName, dailyPrice, adults);
    });
});

// Function to Load Data into Table with Pagination
function loadRoomTypeData(pageNumber) {
    const pageSize = 7;
    fetch(`https://localhost:7286/api/Controller_Admin/GetlistRoomType?pageSize=${pageSize}&pageNumber=${pageNumber}`)
        .then(response => response.json())
        .then(data => {
            const tbody = document.querySelector("#RoomTypeTable tbody");
            tbody.innerHTML = "";
            if (!data || !Array.isArray(data)) {
                console.error("Invalid data format:", data);
                return;
            }
            data.forEach((item) => {
                const tr = document.createElement("tr");
                tr.innerHTML = `
                    <td>${item.id}</td>
                    <td>${item.roomTypeName}</td>
                    <td>${item.pricePerNight}</td>
                    <td>${item.quantityAdult}</td>
                    <td>
                        <button class="btn btn-sm btn-warning" onclick="openEditRoomTypeModal(${item.id}, '${item.roomTypeName}', ${item.pricePerNight}, ${item.quantityAdult})">Edit</button>
                        <button class="btn btn-sm btn-danger" onclick="deleteRoomType(${item.id})">Delete</button>
                    </td>
                `;
                tbody.appendChild(tr);
            });

            // Update pagination controls
            document.querySelector("#pageNum").textContent = pageNumber;
        })
        .catch(error => console.error("Error fetching RoomType data:", error));
}

// Function to Create RoomType
function createRoomType(roomTypeName, dailyPrice, adults) {
    fetch("https://localhost:7286/api/Controller_Admin/CreateRoomType", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ roomTypeName: roomTypeName,description: "string", pricePerNight: dailyPrice, quantityAdult: adults })
    })
    .then(response => response.json())
    .then(result => {
        if (result.status!=200) {
            throw new Error(result.message || "Error creating RoomType");
        }
        alert(result.message);
        loadRoomTypeData(1);
        const createModal = bootstrap.Modal.getInstance(document.getElementById('createRoomTypeModal'));
        createModal.hide();
    })
    .catch(error => console.error("Error:", error));
}

// Function to Open Edit Modal
function openEditRoomTypeModal(id, roomTypeName, pricePerNight, quantityAdult) {
    document.querySelector("#editRoomTypeModal input[name='id']").value = id;
    document.querySelector("#editRoomTypeModal input[name='roomTypeName']").value = roomTypeName;
    document.querySelector("#editRoomTypeModal input[name='dailyPrice']").value = pricePerNight;
    document.querySelector("#editRoomTypeModal input[name='adults']").value = quantityAdult;
    const editModal = new bootstrap.Modal(document.getElementById('editRoomTypeModal'));
    editModal.show();
}


// Function to Update RoomType
function updateRoomType(id, roomTypeName, dailyPrice, adults) {
    fetch("https://localhost:7286/api/Controller_Admin/UpdateRoomType", {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ id: id, roomTypeName: roomTypeName, description: "string", pricePerNight: dailyPrice, quantityAdult: adults })
    })
    .then(response => response.json())
    .then(result => {
        if (result.status!=200) {
            throw new Error(result.message || "Error updating RoomType");
        }
        alert(result.message);
        loadRoomTypeData(1);
        const editModal = bootstrap.Modal.getInstance(document.getElementById('editRoomTypeModal'));
        editModal.hide();
    })
    .catch(error => console.error("Error:", error));
}

// Function to Delete RoomType
function deleteRoomType(id) {
    if (confirm("Bạn có chắc chắn muốn xóa loại phòng này ?")) {
        fetch(`https://localhost:7286/api/Controller_Admin/DeleteRoomType?service=${id}`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json"
            }
        })
        .then(response => response.json())
        .then(result => {
            if (result.status!=200) {
                throw new Error(result.message || "Error deleting RoomType");
            }
            alert(result.message);
            loadRoomTypeData(1);
        })
        .catch(error => console.error("Error:", error));
    }
}

// Pagination Functions
function nextPage() {
    const currentPage = parseInt(document.querySelector("#pageNum").textContent);
    loadRoomTypeData(currentPage + 1);
}

function prevPage() {
    const currentPage = parseInt(document.querySelector("#pageNum").textContent);
    if (currentPage > 1) {
        loadRoomTypeData(currentPage - 1);
    }
}

// HTML for Create and Edit Modals
const createRoomTypeModalHTML = `
<div class="modal fade" id="createRoomTypeModal" tabindex="-1" aria-labelledby="createRoomTypeModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="createRoomTypeModalLabel">Thêm loại phòng</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="roomTypeName" class="form-label">Tên loại phòng</label>
                    <input type="text" class="form-control" name="roomTypeName" required>
                </div>
                <div class="mb-3">
                    <label for="dailyPrice" class="form-label">Giá ngày</label>
                    <input type="number" class="form-control" name="dailyPrice" required>
                </div>
                <div class="mb-3">
                    <label for="adults" class="form-label">Số người lớn</label>
                    <input type="number" class="form-control" name="adults" required>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary">Save changes</button>
            </div>
        </div>
    </div>
</div>`;

document.body.insertAdjacentHTML('beforeend', createRoomTypeModalHTML);

const editRoomTypeModalHTML = `
<div class="modal fade" id="editRoomTypeModal" tabindex="-1" aria-labelledby="editRoomTypeModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="editRoomTypeModalLabel">Sửa loại phòng</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" name="id">
                <div class="mb-3">
                    <label for="roomTypeName" class="form-label">Tên loại phòng</label>
                    <input type="text" class="form-control" name="roomTypeName" required>
                </div>
                <div class="mb-3">
                    <label for="dailyPrice" class="form-label">Giá ngày</label>
                    <input type="number" class="form-control" name="dailyPrice" required>
                </div>
                <div class="mb-3">
                    <label for="adults" class="form-label">Số người lớn</label>
                    <input type="number" class="form-control" name="adults" required>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary">Save changes</button>
            </div>
        </div>
    </div>
</div>`;

document.body.insertAdjacentHTML('beforeend', editRoomTypeModalHTML);
