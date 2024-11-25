namespace QLKS_v1.Payload.Requests.Request_User
{
    public class Request_Register
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? NumberPhone { get; set; }
        public IFormFile? UrlAvt { get; set; }
        public string? Address { get; set; }
    }
}
