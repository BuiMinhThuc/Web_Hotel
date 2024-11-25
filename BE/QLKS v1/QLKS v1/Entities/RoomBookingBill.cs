namespace QLKS_v1.Entities
{
    public class RoomBookingBill:BaseEntity
    {
        public int BookingId {  get; set; }
        public int RoomTypeId {  get; set; }
        public int Quantity { get; set; }
        public Booking? Booking { get; set; }
        public RoomType? RoomType { get; set; }
        public ICollection<RoomNameBill>? RoomNameBills { get; set; }
    }
}
