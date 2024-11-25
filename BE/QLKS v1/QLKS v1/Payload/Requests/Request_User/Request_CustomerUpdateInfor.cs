namespace QLKS_v1.Payload.Requests.Request_User
{
    public class Request_CustomerUpdateInfor
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string NumberPhone { get; set; }
        public IFormFile? UrlAvatar { get; set; } 
        public string Address { get; set; }
        public string? Gender { get; set; }
    }
}
