// JavaScript for CRUD operations on TypeService Table with Pagination Support

document.addEventListener("DOMContentLoaded", function() {
    loadData(1);

    // Add Service Event
    document.querySelector("#createTypeServiceModal .btn-primary").addEventListener("click", function() {
        const nameType = document.querySelector("#createTypeServiceModal input[name='nameType']").value;
        createTypeService(nameType);
    });

    // Edit Service Event
    document.querySelector("#editTypeServiceModal .btn-primary").addEventListener("click", function() {
        const id = document.querySelector("#editTypeServiceModal input[name='id']").value;
        const nameType = document.querySelector("#editTypeServiceModal input[name='nameType']").value;
        updateTypeService(id, nameType);
    });
});

// Function to Load Data into Table with Pagination
function loadData(pageNumber) {
    const pageSize = 7;
    fetch(`https://localhost:7286/api/Controller_Admin/AdminGetChoonseTYpeService?pageSize=${pageSize}&pageNumber=${pageNumber}`)
        .then(response => response.json())
        .then(data => {
            const tbody = document.querySelector("#TypeServiceTable tbody");
            tbody.innerHTML = "";
            if (!data.result || !Array.isArray(data.result)) {
                console.error("Invalid data format:", data);
                return;
            }
            data.result.forEach((item) => {
                const tr = document.createElement("tr");
                tr.innerHTML = `
                    <td>${item.id}</td>
                    <td>${item.typeServiceName}</td>
                    <td>
                        <button class="btn btn-sm btn-warning" onclick="openEditModal(${item.id}, '${item.typeServiceName}')">Sửa</button>
                        <button class="btn btn-sm btn-danger" onclick="deleteTypeService(${item.id})">Xóa</button>
                    </td>
                `;
                tbody.appendChild(tr);
            });

            // Update pagination controls
            document.querySelector("#pageNum").textContent = pageNumber;
        })
        .catch(error => console.error("Error fetching TypeService data:", error));
}

// Function to Create TypeService
function createTypeService(nameType) {
    fetch("https://localhost:7286/api/Controller_Admin/CreateTypeService", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ nameType: nameType })
    })
    .then(response => response.json())
    .then(result => {
        if (result.status!=200) {
            throw new Error(result.message || "Error creating TypeService");
        }
        alert(result.message);
        loadData(1);
        const createModal = bootstrap.Modal.getInstance(document.getElementById('createTypeServiceModal'));
        createModal.hide();
    })
    .catch(error => console.error("Error:", error));
}

// Function to Open Edit Modal
function openEditModal(id, nameType) {
    document.querySelector("#editTypeServiceModal input[name='id']").value = id;
    document.querySelector("#editTypeServiceModal input[name='nameType']").value = nameType;
    const editModal = new bootstrap.Modal(document.getElementById('editTypeServiceModal'));
    editModal.show();
}

// Function to Update TypeService
function updateTypeService(id, nameType) {
    fetch("https://localhost:7286/api/Controller_Admin/UpdateTypeService", {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ id: id, nameType: nameType })
    })
    .then(response => response.json())
    .then(result => {
        if (result.status!=200) {
            throw new Error(result.message || "Error updating TypeService");
        }
        alert(result.message);
        loadData(1);
        const editModal = bootstrap.Modal.getInstance(document.getElementById('editTypeServiceModal'));
        editModal.hide();
    })
    .catch(error => console.error("Error:", error));
}

// Function to Delete TypeService
function deleteTypeService(id) {
    if (confirm("Bạn có chắc chắn muốn xóa loại dịch vụ này?")) {
        fetch(`https://localhost:7286/api/Controller_Admin/DeleteTypeService?service=${id}`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json"
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error("Error deleting TypeService");
            }
            alert("Xóa thành công !")
            loadData(1);
        })
        .catch(error => console.error("Error:", error));
    }
}

// Pagination Functions
function nextPage() {
    const currentPage = parseInt(document.querySelector("#pageNum").textContent);
    loadData(currentPage + 1);
}

function prevPage() {
    const currentPage = parseInt(document.querySelector("#pageNum").textContent);
    if (currentPage > 1) {
        loadData(currentPage - 1);
    }
}

// HTML for Create and Edit Modals
const createModalHTML = `
<div class="modal fade" id="createTypeServiceModal" tabindex="-1" aria-labelledby="createTypeServiceModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="createTypeServiceModalLabel">Thêm loại dịch vụ</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="nameType" class="form-label">Tên loại dịch vụ</label>
                    <input type="text" class="form-control" name="nameType" required>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary">Thêm</button>
            </div>
        </div>
    </div>
</div>`;

document.body.insertAdjacentHTML('beforeend', createModalHTML);

const editModalHTML = `
<div class="modal fade" id="editTypeServiceModal" tabindex="-1" aria-labelledby="editTypeServiceModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="editTypeServiceModalLabel">Sửa loại dịch vụ</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" name="id">
                <div class="mb-3">
                    <label for="nameType" class="form-label">Tên loại dịch vụ</label>
                    <input type="text" class="form-control" name="nameType" required>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary">Sửa</button>
            </div>
        </div>
    </div>
</div>`;

document.body.insertAdjacentHTML('beforeend', editModalHTML);
