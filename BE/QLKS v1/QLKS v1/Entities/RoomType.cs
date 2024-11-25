using System.ComponentModel.DataAnnotations;

namespace QLKS_v1.Entities
{
    public class RoomType:BaseEntity
    {
        [Required, MaxLength(50)]
        public string RoomTypeName { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal PricePerNight { get; set; }
        public int? QuantityAdult {  get; set; }

        public ICollection<Room>? Rooms { get; set; }
        public ICollection<RoomBookingBill>? RoomBookingBills { get; set; }
    }
}
