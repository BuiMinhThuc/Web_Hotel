using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Service;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_Service
    {
       // Task<ResponseBase> CreateService(RequestDecompressionServiceExtensions request);
        IQueryable<DTO_ChoonseService> GetChoonseService();
        IQueryable<DTO_Service> GetAllService(int pageSize, int pageNumber);
        Task<ResponseBase> CreateService(Request_CreateService request);
        Task<ResponseBase> UpdateService(Request_UpdateService request);
        Task<ResponseBase> DeleteService(int Id);
    }
}
