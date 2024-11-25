using QLKS_v1.Entities;

namespace QLKS_v1.Payload.Requests.Request_BookingService
{
    public class Request_CreateBookingService
    {
        public int? UserId { get; set; }
        public string RoomName { get; set; }
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
    }
}
