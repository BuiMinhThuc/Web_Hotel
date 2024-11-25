using iText.StyledXmlParser.Node;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_BookingService;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_BookingService
    {
        Task<ResponseBase> CreateBookingService(Request_CreateBookingService request);
        Task<ResponseBase> UpdateBookingService(int billServiceId);
        Task<ResponseBase> DeleteBookingService(int request);
        IQueryable<DTO_BookingService> GetListBookingService(int pageSize,int pageNumber);
        IQueryable<DTO_BookingService> GetListBookingServiceForCase(string Type,int pageSize,int pageNumber);
        Task<IQueryable<DTO_BookingService>> GetListBookingServiceForUser(int UserId,int pageSize,int pageNumber);
        
    }
}
