using QLKS_v1.Entities;
using QLKS_v1.Payload.Requests.Request_Promotion;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_Promotion
    {
        IQueryable<Promotion> getListPromotion();
        Task<IQueryable<Promotion>> getListPromotionAdmin(int pageSize, int pageNumber);

        Task<ResponseBase> CreatePromotion(Request_CreatePromotion request);
        Task<ResponseBase> UpdatePromotion(Request_UpdatePromotion request);
        Task<ResponseBase> DeletePromotion(int request);
    }
}
