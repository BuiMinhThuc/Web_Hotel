using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Equipment;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_Equiment
    {
        Task<IQueryable<DTO_EquementForId>> GetListEquipmentForId(int Id);
        Task<ResponseBase> CreateEquipment(Request_CreateEquipment request);
        Task<ResponseBase> UpdateEquipment(Request_UpdateEquipment request);
        Task<ResponseBase> DeleteEquipment(int request);
    }
}
