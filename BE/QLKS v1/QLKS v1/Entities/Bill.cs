using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLKS_v1.Entities
{
    public class Bill : BaseEntity
    {
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public Booking? Booking { get; set; }
        public bool Status { get; set; } = false;
        public string? ContentPay { get; set; } 
        public string? ContentPayAll { get; set; } 
        public decimal? TotalPrice { get; set; }
        public decimal? PayOk { get; set; }
        public ICollection<BillService>? BillServices { get; set; }
        public DateTime? CreateTime { get; set; }=DateTime.Now;
        public User? User { get; set; }

    }
}
   
