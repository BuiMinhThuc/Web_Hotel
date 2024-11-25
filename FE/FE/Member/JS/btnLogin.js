
function getUserNameFromToken() {
    const token = localStorage.getItem('token');
    if (!token) {
        document.getElementById("imgAvt").style.display ="none";
        return "Đăng nhập";
    }

    try {
        // Token gồm 3 phần ngăn cách bởi dấu chấm: Header.Payload.Signature
        const payloadBase64 = token.split('.')[1]; // Lấy phần Payload (phần thứ 2)
        const payload = JSON.parse(atob(payloadBase64)); // Giải mã Base64 và parse thành JSON
        fetchImageUrl(payload.Id)
        return payload.Username; // Trả về trường "Username"
    } catch (error) {
        console.error('Lỗi khi giải mã token:', error);
        return null;
    }
}
async function fetchImageUrl(Id) {
    try {
      // Gọi API để lấy URL ảnh
      let response = await fetch(`https://localhost:7286/GetUrlAvt${Id}`);
      
      let imageUrl = await response.text();

  

      // Gán URL ảnh vào thuộc tính src của thẻ img
      document.getElementById("imgAvt").src = imageUrl;

    } catch (error) {
      console.error('There was a problem with the fetch operation:', error);
    }
  }

        const btnUserLogin = document.getElementById("btnLogin")
        const username = getUserNameFromToken();
        if(username != null) {
                btnUserLogin.classList.remove('btn-outline-success')
                btnUserLogin.classList.add('btn-success')
                btnUserLogin.textContent=username
                //document.getElementById("imgAvt").src

        }
        if(btnUserLogin.textContent=="Đăng nhập"){
            document.getElementById("btnLogin").addEventListener("click", function() {
                window.location.href = "/FE/Authen/Login.html";
              });
        }
    
    
    