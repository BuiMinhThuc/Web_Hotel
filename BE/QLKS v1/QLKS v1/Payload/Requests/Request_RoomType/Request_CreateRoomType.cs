using System.ComponentModel.DataAnnotations;

namespace QLKS_v1.Payload.Requests.Request_RoomType
{
    public class Request_CreateRoomType
    {
        public string RoomTypeName { get; set; }

        public string Description { get; set; }

      
        public decimal PricePerNight { get; set; }
        public int? QuantityAdult { get; set; }
    }
}
