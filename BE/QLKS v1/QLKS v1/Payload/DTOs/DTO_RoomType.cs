using System.ComponentModel.DataAnnotations;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_RoomType
    {
        public int Id { get; set; }
        public string RoomTypeName { get; set; }

        public string Description { get; set; }

  
        public decimal PricePerNight { get; set; }
        public int? QuantityAdult { get; set; }
    }
}
