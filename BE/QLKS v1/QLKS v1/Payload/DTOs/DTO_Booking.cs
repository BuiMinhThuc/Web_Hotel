using QLKS_v1.Entities;
using QLKS_v1.Payload.Requests.Request_BookingRoom;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_Booking
    {
        public int ID {  get; set; }
        public int billId {  get; set; }
        public string NumberPhone { get; set; }
        public string CustomerName{ get; set; }
        public string CardNumber{ get; set; }
        public int CustomerID { get; set; }
        public List<Requesr_ListRoom> listRoomType { get; set; }
        public List<string> listRoomName { get; set; }
        public int RoomID { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int? NumberOfChild { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
