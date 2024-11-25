using QLKS_v1.Entities;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_BookingService
    {
        public int Id {  get; set; }
        public int BillId { get; set; }

        public int Quantity { get; set; }
        public string RoomName { get; set; }

        public string ServiceName { get; set; }
        public DateTime? CreateTime { get; set; }
        public bool? Status { get; set; }
    }
}
