document.addEventListener("DOMContentLoaded", function () {
    loadWorkShiftData(1);
    fetchStaffList();

    // Add Work Shift Event
    document.querySelector("#createWorkShiftModal .btn-primary").addEventListener("click", function () {
        handleWorkShiftSubmit("create");
    });

    // Edit Work Shift Event
    document.querySelector("#editWorkShiftModal .btn-primary").addEventListener("click", function () {
        handleWorkShiftSubmit("edit");
    });
});


// Function to open edit modal and preselect the nearest matching user
// Function to open edit modal and preselect the matching user by ID
function openEditWorkShiftModal(id, name, date, startTime, endTime, shiftTypes) {
    const modal = document.getElementById("editWorkShiftModal");
    modal.querySelector("input[name='id']").value = id;
    modal.querySelector("input[name='date']").value = date.split('T')[0];

    // Định dạng thời gian theo giờ địa phương để hiển thị
    const formattedStartTime = new Date(startTime).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: false });
    const formattedEndTime = new Date(endTime).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: false });

    modal.querySelector("input[name='startTime']").value = formattedStartTime;
    modal.querySelector("input[name='endTime']").value = formattedEndTime;
    modal.querySelector("select[name='shiftTypes']").value = shiftTypes;

    // Tìm và chọn mục trong select có tên nhân viên gần giống nhất với `name`
    selectClosestMatchingUser(name);

    // Hiển thị modal
    const bootstrapModal = new bootstrap.Modal(modal);
    bootstrapModal.show();
}



function handleWorkShiftSubmit(action) {
    const modalId = action === "create" ? "#createWorkShiftModal" : "#editWorkShiftModal";
    const user = document.querySelector(`${modalId} select[name='user']`).value;
    const date = document.querySelector(`${modalId} input[name='date']`).value;
    const startTime = document.querySelector(`${modalId} input[name='startTime']`).value;
    const endTime = document.querySelector(`${modalId} input[name='endTime']`).value;
    const shiftTypes = document.querySelector(`${modalId} select[name='shiftTypes']`).value;

    // Kết hợp ngày và giờ thành chuỗi ISO UTC mà không chuyển đổi múi giờ
    const startDateTime = `${date}T${startTime}:00Z`;
    const endDateTime = `${date}T${endTime}:00Z`;
    const dateISO = `${date}T00:00:00Z`; // Chỉ ngày, không có giờ

    if (action === "create") {
        createOrUpdateWorkShift("POST", user, dateISO, startDateTime, endDateTime, shiftTypes, modalId);
    } else {
        const id = document.querySelector(`${modalId} input[name='id']`).value;
        createOrUpdateWorkShift("PUT", user, dateISO, startDateTime, endDateTime, shiftTypes, modalId, id);
    }
}



// Function to Load Data into Work Shift Table with Pagination
function loadWorkShiftData(pageNumber) {
    fetch(`https://localhost:7286/api/Controller_Member/getListStaffShift?pageSize=7&pageNumber=${pageNumber}`)
        .then(response => response.json())
        .then(data => {
            const tbody = document.querySelector("#staffShiftTable tbody");
            tbody.innerHTML = "";
            if (!data || !Array.isArray(data)) {
                console.error("Invalid data format:", data);
                return;
            }
            data.forEach(item => tbody.appendChild(createTableRow(item)));
            document.querySelector("#pageNumStaffShift").textContent = pageNumber;
        })
        .catch(error => console.error("Error fetching Work Shift data:", error));
}

// Helper function to create table row
function createTableRow(item) {
    const tr = document.createElement("tr");
    tr.innerHTML = `
        <td>${item.id}</td>
        <td>${item.userName}</td>
        <td>${item.numberPhone}</td>
        <td>${item.date.split('T')[0]}</td>
        <td>${item.startTime.split('T')[1].slice(0, 5)}</td>
        <td>${item.endTime.split('T')[1].slice(0, 5)}</td>
        <td>${item.shiftTypes}</td>
        <td>
            <button class="btn btn-sm btn-warning" onclick="openEditWorkShiftModal(${item.id}, '${item.name}', '${item.date}', '${item.startTime}', '${item.endTime}', '${item.shiftTypes}')">Edit</button>
            <button class="btn btn-sm btn-danger" onclick="deleteWorkShift(${item.id})">Delete</button>
        </td>
    `;
    return tr;
}

// Function to Create or Update Work Shift
function createOrUpdateWorkShift(method, user, date, startDateTime, endDateTime, shiftTypes, modalId, id = null) {
    const url = method === "POST"
        ? "https://localhost:7286/api/Controller_Admin/CreateStaffShift"
        : `https://localhost:7286/api/Controller_Admin/UpdateStaffShift`;

    // Tạo đối tượng dữ liệu để gửi
    const data = {
        user,
        date,
        startTime: startDateTime,
        endTime: endDateTime,
        shiftTypes
    };

    // Nếu có ID (khi cập nhật), thêm ID vào đối tượng
    if (id !== null) {
        data.id = id;
    }
console.log(id)
    fetch(url, {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    })
    .then(response => response.json())
    .then(result => {
        if (result.status !== 200) throw new Error(result.message || "Error processing Work Shift");
        alert(result.message);
        loadWorkShiftData(1);
        bootstrap.Modal.getInstance(document.querySelector(modalId)).hide();
    })
    .catch(error => console.error("Error:", error));
}


// Function to Delete Work Shift
function deleteWorkShift(id) {
    if (confirm("Are you sure you want to delete this Work Shift?")) {
        fetch(`https://localhost:7286/api/Controller_Admin/DeleteStaffShift?service=${id}`, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" }
        })
        .then(response => response.json())
        .then(result => {
            if (result.status !== 200) throw new Error(result.message || "Error deleting Work Shift");
            alert(result.message);
            loadWorkShiftData(1);
        })
        .catch(error => console.error("Error:", error));
    }
}

// Pagination Functions for Work Shift
function nextStaffShiftPage() {
    const currentPage = parseInt(document.querySelector("#pageNumStaffShift").textContent);
    loadWorkShiftData(currentPage + 1);
}

function prevStaffShiftPage() {
    const currentPage = parseInt(document.querySelector("#pageNumStaffShift").textContent);
    if (currentPage > 1) loadWorkShiftData(currentPage - 1);
}

// Fetch Staff List and Populate Select Options
function fetchStaffList() {
    fetch("https://localhost:7286/api/Controller_Member/getListStaff?pageSize=100&pageNumber=1")
        .then(response => response.json())
        .then(data => {
            const userSelectElements = document.querySelectorAll("#createWorkShiftModal select[name='user'], #editWorkShiftModal select[name='user']");
            userSelectElements.forEach(select => select.innerHTML = ""); // Clear existing options
            if (!data || !Array.isArray(data)) return console.error("Invalid staff data format:", data);

            data.forEach(staff => {
                const option = document.createElement("option");
                option.value = staff.id;
                option.textContent = `${staff.id} - ${staff.name}`;
                userSelectElements.forEach(select => select.appendChild(option.cloneNode(true)));
            });
        })
        .catch(error => console.error("Error fetching staff list:", error));
}
// Function to select the closest matching user in the user select element
function selectClosestMatchingUser(name) {
    const userSelect = document.querySelector("#editWorkShiftModal select[name='user']");
    let bestMatch = null;
    let bestScore = -1;

    for (const option of userSelect.options) {
        const optionText = option.textContent || option.innerText;
        const score = similarityScore(optionText, name); // Calculate similarity score

        if (score > bestScore) {
            bestScore = score;
            bestMatch = option;
        }
    }

    if (bestMatch) {
        bestMatch.selected = true;
    }
}

// Helper function to calculate similarity score between two strings
function similarityScore(str1, str2) {
    str1 = str1.toLowerCase();
    str2 = str2.toLowerCase();

    const length = Math.min(str1.length, str2.length);
    let matches = 0;

    for (let i = 0; i < length; i++) {
        if (str1[i] === str2[i]) {
            matches++;
        }
    }

    return matches / Math.max(str1.length, str2.length);
}
// HTML for Create and Edit Modals for Work Shift
const createWorkShiftModalHTML = `
<div class="modal fade" id="createWorkShiftModal" tabindex="-1" aria-labelledby="createWorkShiftModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="createWorkShiftModalLabel">Thêm lịch làm việc</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="user" class="form-label">Nhân viên</label>
                    <select class="form-control" name="user" required></select>
                </div>
                <div class="mb-3">
                    <label for="date" class="form-label">Ngày</label>
                    <input type="date" class="form-control" name="date" required>
                </div>
                <div class="mb-3">
                    <label for="startTime" class="form-label">Giờ bắt đầu</label>
                    <input type="time" class="form-control" name="startTime" required>
                </div>
                <div class="mb-3">
                    <label for="endTime" class="form-label">Giờ kết thúc</label>
                    <input type="time" class="form-control" name="endTime" required>
                </div>
               <div class="mb-3">
                    <label for="shiftTypes" class="form-label">Ca làm</label>
                    <select class="form-control" name="shiftTypes" required>
                        <option value="Sáng">Sáng</option>
                        <option value="Trưa">Trưa</option>
                        <option value="Chiều">Chiều</option>
                        <option value="Tối">Tối</option>
                        <option value="Đêm">Đêm</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary">Save changes</button>
            </div>
        </div>
    </div>
</div>`;

const editWorkShiftModalHTML = `
<div class="modal fade" id="editWorkShiftModal" tabindex="-1" aria-labelledby="editWorkShiftModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="editWorkShiftModalLabel">Sửa lịch làm việc</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" name="id">
                <div class="mb-3">
                    <label for="user" class="form-label">Nhân viên</label>
                    <select class="form-control" name="user" required></select>
                </div>
                <div class="mb-3">
                    <label for="date" class="form-label">Ngày</label>
                    <input type="date" class="form-control" name="date" required>
                </div>
                <div class="mb-3">
                    <label for="startTime" class="form-label">Giờ bắt đầu</label>
                    <input type="time" class="form-control" name="startTime" required>
                </div>
                <div class="mb-3">
                    <label for="endTime" class="form-label">Giờ kết thúc</label>
                    <input type="time" class="form-control" name="endTime" required>
                </div>
                <div class="mb-3">
                    <label for="shiftTypes" class="form-label">Ca làm</label>
                    <select class="form-control" name="shiftTypes" required>
                        <option value="Sáng">Sáng</option>
                        <option value="Trưa">Trưa</option>
                        <option value="Chiều">Chiều</option>
                        <option value="Tối">Tối</option>
                        <option value="Đêm">Đêm</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                <button type="button" class="btn btn-primary">Save changes</button>
            </div>
        </div>
    </div>
</div>`;

document.body.insertAdjacentHTML('beforeend', createWorkShiftModalHTML);
document.body.insertAdjacentHTML('beforeend', editWorkShiftModalHTML);
