namespace QLKS_v1.Handle
{
    public class EmailRequest
    {
        public string Email { get; set; }      // Địa chỉ email của người nhận
        public string Subject { get; set; }    // Chủ đề của email
        public string Content { get; set; }    // Nội dung chính của email
        public string QrCodeUrl { get; set; }  // Đường dẫn mã QR (URL của hình ảnh mã QR)
    }
}
