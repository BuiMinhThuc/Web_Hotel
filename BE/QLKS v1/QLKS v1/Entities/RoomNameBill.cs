namespace QLKS_v1.Entities
{
    public class RoomNameBill:BaseEntity
    {
        public int RoomBookingBillId {  get; set; }
        public string NameRoom { get; set; }
        public RoomBookingBill? RoomBookingBill { get; set; }
    }
}
