using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_CustomerFeedBack;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_FeedBackCustomer : IService_FeedBackCustomer
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_CustomerFeedBack converter_CustomerFeedBack;
        private readonly ResponseBase responseBase;

        public Service_FeedBackCustomer(AppDbContext dbContext, Converter_CustomerFeedBack converter_CustomerFeedBack, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.converter_CustomerFeedBack = converter_CustomerFeedBack;
            this.responseBase = responseBase;
        }

        public IQueryable<DTO_FeedBackCustomer> GetlistFeedBack(int pageSize, int pageNumber)
        {
            return dbContext.customerFeedbacks.OrderByDescending(x=>x.Id).Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x=>converter_CustomerFeedBack.EntityToDTO(x)).AsQueryable();
        }
       

        public async Task<ResponseBase> CreateFeedBack(Request_CreateFeedBack request)
        {
            if (request.Content.Length < 2 )
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Vui lòng nhập nội dung !");
            }
            var feedBack = new CustomerFeedback();
            feedBack.UserId = request.UserId;
            feedBack.Content = request.Content;
            feedBack.rate = request.Rate==0? 5 : request.Rate; 
            dbContext.customerFeedbacks.Add(feedBack);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Phản hồi thành công !");
        }
    }
}
