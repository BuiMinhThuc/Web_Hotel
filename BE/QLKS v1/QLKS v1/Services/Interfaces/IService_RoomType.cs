using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_RoomType;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_RoomType
    {
        IQueryable<DTO_getRoomTypeName> getListRoomTypenam();
        IQueryable<DTO_RoomType> AdmingetRoomType(int pageSize, int pageNumber );
        Task<DTO_TypeRoomHot> getListRoomTypeNameHot();
        Task<ResponseBase> CreateRooomType(Request_CreateRoomType request);
        Task<ResponseBase> UpdateRooomType(Request_UpdateRoomType request);
        Task<ResponseBase> DeleteRooomType(int request);

    }
}
