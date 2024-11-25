using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_TypeService;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_TypeService
    {
        Task<IQueryable<DTO_ChoonseTypeService>> ChooseTypeServices();
        Task<IQueryable<DTO_ChoonseTypeService>> AdminChooseTypeServices(int pageSize, int pageNumber);
        Task<ResponseBase> CreateTypeService(Request_CreateServiceType request);
        Task<ResponseBase> UpdateTypeService(Request_UpdateTypeService request);
        Task<ResponseBase> DeleteTypeService(int request);
    }
}
