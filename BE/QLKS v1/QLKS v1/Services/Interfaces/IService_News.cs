using QLKS_v1.Entities;
using QLKS_v1.Payload.Requests.Request_News;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_News
    {
        IQueryable<New> GetNews();
        public IQueryable<New> GetNewsPT(int? pageSize, int? pageNumber);
        Task<ResponseBase> CreateNews(Request_CreateNews request);
        Task<ResponseBase> UpdateNews(Request_UpdateNews request);
        Task<ResponseBase> DeleteNews(int request);
    }
}
