using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_CustomerFeedBack
    {
        private readonly AppDbContext dbContext;

        public Converter_CustomerFeedBack(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DTO_FeedBackCustomer EntityToDTO(CustomerFeedback customerFeedback)
        {
            var user = dbContext.users.FirstOrDefault(x => x.Id == customerFeedback.UserId);
            return new DTO_FeedBackCustomer
            {
                NameCustomer=user.FullName,
                UrlAvatarCustomer=user.UrlAvatar,
                Content=customerFeedback.Content,
                Rate=customerFeedback.rate
            };
        }



    }
}
