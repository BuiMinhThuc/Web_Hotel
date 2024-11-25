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