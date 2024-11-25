using QLKS_v1.Entities;

namespace QLKS_v1.Payload.Requests.Request_User
{
    public class Request_UpdateUser
    {
        public int id {  get; set; }
        public string UserName { get; set; }
     
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? NumberPhone { get; set; }
        public IFormFile? UrlAvatar { get; set; }
        public string? Address { get; set; }
        public string? RoleName { get; set; } 
        public bool? IsActice { get; set; }
    }
}
