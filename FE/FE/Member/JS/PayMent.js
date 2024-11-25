 // Lấy URL hiện tại của trang
 const urlParams = new URLSearchParams(window.location.search);

 // Lấy giá trị của tham số 'checkPay'
 const checkPay = urlParams.get('checkPay');

 // Kiểm tra giá trị và thực hiện hành động tùy theo giá trị đó
 if (checkPay === "True") {
     alert("Thanh toán thành công! Quý khách kiểm tra Email để biết thêm thông tin !"); // Hiển thị thông báo cho người dùng
     urlParams.delete('checkPay');

                // Tạo URL mới không chứa tham số 'checkPay'
                const newUrl = window.location.pathname + '?' + urlParams.toString();

                // Sử dụng history.replaceState để thay thế URL mà không tải lại trang
                window.history.replaceState({}, '', newUrl);
     //window.location.href = "/hometesst.html"
 } else {
     console.log("Không có thông tin thanh toán");
 }