namespace QLKS_v1.Payload.Requests.Request_BookingRoom
{
    public class Request_ListRoomType
    {
        public int RoomTypeId {  get; set; }
        public string RoomTypeName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceOfOne {  get; set; }
    }
}
