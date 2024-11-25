using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Staff;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_Staff
    {
        Task<IQueryable<DTO_Staff>> getListStaff(int pageSize, int pageNumber);
        Task<ResponseBase> UpdateStaff(Request_UpdateStaff request);
        Task<ResponseBase> DeleteStaff(int Id);
    }
}
