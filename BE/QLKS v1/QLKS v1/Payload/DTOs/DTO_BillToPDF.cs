using QLKS_v1.Payload.Requests.Request_BookingRoom;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_BillToPDF
    {
       
            public int Id { get; set; }
            public int UserId { get; set; }
            public string NameCustomer { get; set; }
            public string NameStaff { get; set; }
            public string NumberPhone {  get; set; }

            public List<Request_ListRoomType> ListTypeRoom { get; set; }
        public int? child {  get; set; }
            public List<Request_ListRoomNamePDF> ListRoomName {  get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public List<DTO_BillServiceForPDF> ServiceBill {  get; set; }
            public decimal? TotalPrice { get; set; }

        
    }
}
