const baseURL = "https://localhost:7286/api/Controller_Member";
const adminURL = "https://localhost:7286/api/Controller_Admin";
let currentPage = 1;
const pageSize = 7;

document.addEventListener("DOMContentLoaded", () => {
    fetchServiceData();
    setupPagination();
    loadServiceTypes();
});

// Hàm lấy dữ liệu từ API theo trang
async function fetchServiceData(pageNumber = currentPage) {
    try {
        const response = await fetch(`${baseURL}/getAllService?pageSize=${pageSize}&pageNumber=${pageNumber}`);
        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
        const data = await response.json();
        populateServiceTable(data);
    } catch (error) {
        console.error("Có lỗi xảy ra khi lấy dữ liệu từ API:", error);
    }
}

// Hàm đổ dữ liệu vào bảng
function populateServiceTable(serviceData) {
    const serviceTableBody = document.querySelector("#serviceTable tbody");
    serviceTableBody.innerHTML = serviceData.map((service, index) => `
        <tr>
             <!--<td>${(currentPage - 1) * pageSize + index + 1}</td>-->
            <td>${service.id}</td>
            <td>${service.name}</td>
            <td>${service.typeName}</td>
            <td>${service.price.toLocaleString()} VND</td>
            <td ">
                <button type="button" class="btn-editService btn-edit btn-info btn-sm" data-id="${service.id}">Sửa</button>
                <button class="btn-deleteService btn-delete btn-danger btn-sm" data-id="${service.id}">Xóa</button>
            </td>
        </tr>`).join('');
    document.querySelector("#pageNumService").innerText = currentPage;
    setupDeleteButtons();
    setupEditButtons();
}

// Hàm thiết lập sự kiện phân trang
function setupPagination() {
    document.querySelector("#nextBtnService").addEventListener("click", () => {
        currentPage++;
        fetchServiceData();
    });
    document.querySelector("#prevBtnService").addEventListener("click", () => {
        if (currentPage > 1) {
            currentPage--;
            fetchServiceData();
        }
    });
}
async function fetchServiceTypes(selectId, selectedType = "") {
    try {
        const response = await fetch(`${adminURL}/GetChoonseTYpeService`);
        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
        const data = await response.json();
        const serviceTypeSelect = document.getElementById(selectId);
        serviceTypeSelect.innerHTML = data.result.map(type => `
            <option value="${type.typeServiceName}" ${type.typeServiceName === selectedType ? "selected" : ""}>
                ${type.typeServiceName}
            </option>`).join('');
    } catch (error) {
        console.error("Có lỗi xảy ra khi tải loại dịch vụ:", error);
    }
}

// Hàm tải loại dịch vụ từ API
async function loadServiceTypes() {
    try {
        const response = await fetch(`${adminURL}/GetChoonseTYpeService`);
        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
        const data = await response.json();
        const serviceTypeSelect = document.getElementById("serviceType");
        serviceTypeSelect.innerHTML = data.result.map(type => `
            <option value="${type.typeServiceName}">${type.typeServiceName}</option>`).join('');
    } catch (error) {
        console.error("Có lỗi xảy ra khi tải loại dịch vụ:", error);
    }
}

// Hàm thêm dịch vụ mới
async function addService() {
    const name = document.getElementById('serviceName').value;
    const type = document.getElementById('serviceType').value;
    const price = parseInt(document.getElementById('servicePrice').value);

    try {
        const response = await fetch(`${baseURL}/CreateService`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, price, typeService: type })
        });
        const data = await response.json();
        alert(data.message);
        bootstrap.Modal.getInstance(document.getElementById('modalCreateService')).hide();
            fetchServiceData();
    } catch (error) {
        console.error("Có lỗi xảy ra khi thêm dịch vụ:", error);
    }
}

// Hàm cập nhật dịch vụ
async function updateService() {
    const id = document.getElementById("updateServiceId").value;
    const name = document.getElementById("updateServiceName").value;
    const typeService = document.getElementById("updateServiceType").value;
    const price = parseInt(document.getElementById("updateServicePrice").value);

    try {
        const response = await fetch(`${baseURL}/UpdateService`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ id, name, price, typeService })
        });
        const result = await response.json();
        alert(response.ok ? "Cập nhật dịch vụ thành công!" : `Cập nhật thất bại: ${result.message}`);
        if (response.ok) {
            bootstrap.Modal.getInstance(document.getElementById('modalUpdateService')).hide();
            fetchServiceData();
        }
    } catch (error) {
        console.error("Có lỗi xảy ra khi cập nhật dịch vụ:", error);
    }
}

// Hàm xóa dịch vụ
async function deleteService(serviceId) {
    try {
        const response = await fetch(`${baseURL}/DeleteService?Id=${serviceId}`, { method: 'DELETE' });
        const result = await response.json();
        alert(response.ok ? "Xóa dịch vụ thành công!" : result.message || "Xóa thất bại, vui lòng thử lại!");
        if (response.ok) fetchServiceData();
    } catch (error) {
        console.error("Có lỗi xảy ra khi xóa dịch vụ:", error);
    }
}

// Thiết lập sự kiện cho các nút xóa
function setupDeleteButtons() {
    document.querySelectorAll(".btn-deleteService").forEach(button => {
        button.addEventListener("click", () => {
            if (confirm("Bạn có chắc muốn xóa dịch vụ này không?")) {
                deleteService(button.getAttribute("data-id"));
            }
        });
    });
}

// Thiết lập sự kiện cho các nút sửa
function setupEditButtons() {
    document.querySelectorAll(".btn-editService").forEach(button => {
        button.addEventListener("click", () => {
            const row = button.closest("tr");
            openUpdateModal({
                id: button.getAttribute("data-id"),
                name: row.children[1].textContent,
                typeName: row.children[2].textContent,
                price: parseInt(row.children[3].textContent.replace(/\D/g, ""))
            });
        });
    });
}

// Hàm mở modal cập nhật
function openUpdateModal(service) {
    document.getElementById("updateServiceId").value = service.id;
    document.getElementById("updateServiceName").value = service.name;
    document.getElementById("updateServicePrice").value = service.price;
    fetchServiceTypes("updateServiceType", service.typeName);
    new bootstrap.Modal(document.getElementById('modalUpdateService')).show();
}
