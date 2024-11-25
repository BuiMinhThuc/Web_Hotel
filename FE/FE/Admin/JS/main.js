
function getUserNameFromToken() {
    const token = localStorage.getItem('token');
    
    try {
        // Token gồm 3 phần ngăn cách bởi dấu chấm: Header.Payload.Signature
        const payloadBase64 = token.split('.')[1]; // Lấy phần Payload (phần thứ 2)
        const payload = JSON.parse(atob(payloadBase64)); // Giải mã Base64 và parse thành JSON
        return payload.Username; // Trả về trường "Username"
    } catch (error) {
        console.error('Lỗi khi giải mã token:', error);
        return null;
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
        // if(btnUserLogin.textContent!="Đăng nhập"){
        //     document.getElementById("btnLogin").addEventListener("click", function() {
        //         if(confirm('Bạn có muốn đăng xuất không ?')){
        //           localStorage.removeItem('token')
        //             window.location.href = "/FE/Authen/Login.html";
        //         }
                
        //       });
        // }
  // Lấy tất cả các nút chuyển tab
  const tabButtons = document.querySelectorAll('.tab-button');
  
  // Thêm sự kiện 'click' cho mỗi nút tab
  tabButtons.forEach(button => {
    button.addEventListener('click', function () {
      // Bỏ class 'active' khỏi tất cả các nút tab
      tabButtons.forEach(btn => btn.classList.remove('active'));
      
      // Thêm class 'active' cho tab được chọn
      this.classList.add('active');
      
      // Lấy tên của tab được chọn từ thuộc tính 'data-tab'
      const selectedTab = this.getAttribute('data-tab');
      
      // Ẩn tất cả các nội dung tab
      const allTabs = document.querySelectorAll('.tab');
      allTabs.forEach(tab => tab.classList.remove('active'));
      
      // Hiển thị nội dung của tab được chọn
      const activeTab = document.getElementById(selectedTab);
      if (activeTab) {
        activeTab.classList.add('active');
      }
    });
  });

