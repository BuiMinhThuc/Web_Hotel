namespace QLKS_v1.Payload.DTOs
{
    public class DTO_GetUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? NumberPhone { get; set; }
        public string? UrlAvatar { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public int? Point { get; set; } = 0;
        public int? UserStatusId { get; set; } = 2;
        public int? RoleId { get; set; } = 1;
        public string? RoleName { get; set; } 
        public bool? IsActice { get; set; } = true;
    }
}
