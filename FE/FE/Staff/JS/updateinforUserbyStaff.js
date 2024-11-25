async function FetchAPI(userId) {
    try {
        const response = await fetch(`https://localhost:7286/GetUserbyId${userId}`);

        // Kiểm tra nếu phản hồi không thành công
        if (!response.ok) {
            throw new Error(`Error: ${response.status} - ${response.statusText}`);
        }

        // Kiểm tra nếu phản hồi có dữ liệu trước khi parse JSON
        const responseData = await response.text(); // Lấy dữ liệu dưới dạng chuỗi
        if (!responseData) {
            throw new Error('No data returned from API');
        }

        // Phân tích chuỗi JSON
        const user = JSON.parse(responseData);
        return user.data;
    } catch (error) {
        console.error('Lỗi FetchAPI:', error);
        return null; // Trả về null nếu có lỗi
    }
}
function getPayloadFromToken() {
    const token = localStorage.getItem('token');
    try {
        // Token gồm 3 phần ngăn cách bởi dấu chấm: Header.Payload.Signature
        const payloadBase64 = token.split('.')[1]; // Lấy phần Payload (phần thứ 2)
        const payload = JSON.parse(atob(payloadBase64)); // Giải mã Base64 và parse thành JSON
        return payload// Trả về 

    } catch (error) {
        console.error('Lỗi khi giải mã token:', error);
        return null;
    }
}
async function initializeForm() {
    const payload = getPayloadFromToken();
    if (!payload || !payload.Id) {
        console.error('Không tìm thấy token hợp lệ hoặc ID');
        return;
    }

    const user = await FetchAPI(payload.Id);
    if (!user) {
        console.error('Không lấy được thông tin người dùng');
        return;
    }

    const fullname1 = document.getElementById('Full-Name2');
 
    const numberPhone1 = document.getElementById('NumberPhone2');
   
    const email1 = document.getElementById('Email2');
   
    const address1 = document.getElementById('Address2');
    
    const urlAvatar1 = document.getElementById('UrlAvatar2');

    const gender = document.getElementById('Gender');
  
 

    // Điền dữ liệu người dùng vào form
    fullname1.value = user.fullName;
    fullname1.disabled = true
    numberPhone1.value = user.numberPhone
    numberPhone1.disabled = true
    email1.value = user.email
    email1.disabled = true
    address1.value = user.address
    address1.disabled = true
    urlAvatar1.src = user.urlAvatar
    urlAvatar1.disabled = true
    gender.value = user.gender.trim();  
    gender.disabled = true

    const updatefullName = document.getElementById('update-FullName')
    const updatenumberPhone = document.getElementById('update-NumberPhone')
    const updateEmail = document.getElementById('update-Email')
    const updateAddress = document.getElementById('update-Address')
    const updateuurlAvt = document.getElementById('update-urlAvt')
    const updategenDer = document.getElementById('update-Gender')

    updatefullName.addEventListener('click', function () {
        fullname1.disabled = false
    });
    updatenumberPhone.addEventListener('click', function () {
        numberPhone1.disabled = false
    });
    updateEmail.addEventListener('click', function () {
        email1.disabled = false
    });
    updateAddress.addEventListener('click', function () {
        address1.disabled = false
    });
    updateuurlAvt.addEventListener('click', function () {
        urlAvatar1.disabled = false
    });
    updategenDer.addEventListener('click', function () {
        gender.disabled = false
    });

    const formData = new FormData();

    formData.append('Email', email1.text);
    formData.append('FullName', fullname1.text);
    formData.append('NumberPhone', numberPhone1.text);
    formData.append('Address', address1.text);
    formData.append('Gender', gender.text);
    if (urlAvatar1) {
        formData.append('UrlAvatar', urlAvatar1.files[0]);
    }



    const submit = document.getElementById('BtnupdateinforUserbyCustomer');
    submit.addEventListener('click', async function () {
        // Tạo formData với các giá trị từ các trường sau khi người dùng đã chỉnh sửa
        const formData = new FormData();
        formData.append('Email', email1.value); // Lấy giá trị mới từ input
        formData.append('FullName', fullname1.value);
        formData.append('NumberPhone', numberPhone1.value);
        formData.append('Address', address1.value);
        formData.append('Gender', gender.value);
        
        // Nếu trường UrlAvatar được thay đổi, lấy file mới
        if (urlAvatar1.files.length > 0) {
            console.log("🚀 ~ urlAvatar1:", urlAvatar1)
            formData.append('UrlAvatar', urlAvatar1.files[0]);
        }
    
        try {
            const response = await fetch(`https://localhost:7286/Update${payload.Id}`, {
                method: 'POST',
                body: formData
            });
    
            const result = await response.json();
    
            if (result.status === 200) {
                localStorage.removeItem('token');
                localStorage.setItem('token', result.message);
                console.log("🚀 ~ result.message:", result.message)
                alert("Cập nhật thành công !");
            } else {
                alert(result.message);
            }
        } catch (error) {
            console.error('Lỗi trong quá trình fetch:', error);
        }
    });
    
}




document.addEventListener('DOMContentLoaded', function () {
    // preventDefault();
    initializeForm();


});



