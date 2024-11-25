using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Booking;
using QLKS_v1.Payload.Requests.Request_BookingRoom;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_Booking
    {
        public Task<ResponseObject<DTO_Booking>> CreateBooking(Request_CreateBooking request);
        public Task<ResponseBase> AdminCreateBooking(Request_AdminBooking request);
        public Task<ResponseBase> UpdateBooking(Request_UpdateBooking request);
        public Task<string> DeleteBooking(int request);
        public Task<IQueryable<DTO_Booking>> GetListBooking(int? pageSize,int? pageNumber);

    }
}
