using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;

namespace QLKS_v1.Entities
{
    public class User: BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? NumberPhone {  get; set; }
        public string? UrlAvatar { get; set; }
        public string? Address {  get; set; }
        public int? Point { get; set; } = 0;
        public string? Gender { get; set; } 
        public int? UserStatusId { get; set; } = 2;
        public int? RoleId { get; set; } = 2;
        public bool? IsActice { get; set; } = true;
        public Role Role { get; set; }
        public UserStatus? UserStatus { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<ComfirmEmail>? ConfirmEmails { get; set; }
        public ICollection<StaffShift>? StaffShifts { get; set; }
        public ICollection<CustomerFeedback>? CustomerFeedbacks { get; set; }
        public ICollection<Bill>? Bills { get; set; }
        public ICollection<BillService>? BillServices { get; set; }
    }
}
