using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_StaffShift;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_StaffShift
    {
        IQueryable<DTO_StaffShift> GetListStaffShift(int pageSize,int pageNumber);
        Task<ResponseBase> CreateStaffShift(Request_CreateStaffShift request);
        Task<ResponseBase> UpdateStaffShift(Request_UpdateStaffShift request);
        Task<ResponseBase> DeleteStaffShift(int request);
    }
}
