using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_CustomerFeedBack;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_FeedBackCustomer
    {
        IQueryable<DTO_FeedBackCustomer> GetlistFeedBack(int pageSize, int pageNumber);
        Task<ResponseBase> CreateFeedBack(Request_CreateFeedBack request);
    }
}
