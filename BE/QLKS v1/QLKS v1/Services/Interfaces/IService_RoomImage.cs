using Azure;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_ImageRoom;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_RoomImage
    {
        IQueryable<DTO_ImageRoom> getListAvtRoom();

        Task<List<DTO_ImageRoom>> ListImgOfRoom(int Id);
        Task<ResponseBase> CreateImgeRoom(Request_CreateImgRoom request);
        Task<ResponseBase> UpdateImgeRoom(Request_UpdateImgRoom request);
        Task<ResponseBase> DeletaImgeRoom(int request);
    }
}
