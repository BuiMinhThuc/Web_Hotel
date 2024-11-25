using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Room;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_Room
    {
      IQueryable<DTO_ChooseRoom> getListChooseRoom();
      IQueryable<DTO_Room> getListRoom();
      
    
      IQueryable<DTO_Room> getAllRoom(int pageSize, int pageNumber);

        DTO_ViewChiTietRoom ViewChiTietRoom(string RoomName);
        IQueryable<DTO_Room> GetListRoomforCase(string RoomTypeName, int pageSize, int pageNumber);
        Task<IQueryable<DTO_Room>> getListRoomNull(string RoomTypeName, int pageSize, int pageNumber);
        Task<IQueryable<DTO_Room>> getListRoomNullFull(string RoomTypeName, int pageSize, int pageNumber, DateTime checkIn, DateTime checkOut);
        Task<ResponseBase> CreateRoom(Request_CreateRoom request);
        Task<ResponseBase> UpdateRoom(Request_UpdateRoom request);
        Task<ResponseBase> DeleteRoom(int RoomId);
        Task<int> TypeALone();
        Task<int> TypeDouble();
        Task<int> TypeVip();
    }
}
