using Org.BouncyCastle.Bcpg;

namespace QLKS_v1.Payload.Requests.Request_BookingRoom
{
    public class Request_ListRoomNamePDF
    {
        public int RoomTypeId {  get; set; }
        public string Name { get; set; }
        
    }
}
