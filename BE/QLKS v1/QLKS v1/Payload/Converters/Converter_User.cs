using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_User
    {
        private readonly AppDbContext dbContext;

        public Converter_User(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DTO_GetUser EntityToDTO(User user)
        {
            return new DTO_GetUser
            {
                Id = user.Id,
                FullName = user.FullName,
                Address = user.Address,
                Point = user.Point,
                NumberPhone = user.NumberPhone,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = dbContext.roles.Find(user.RoleId).RoleName,
                UrlAvatar = user.UrlAvatar,
                UserName = user.UserName,
                Gender = user.Gender,
                IsActice = user.IsActice,
                UserStatusId = user.UserStatusId,
            };
        }
    }
}
