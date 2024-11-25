using QLKS_v1.Entities;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_Bill
    {
        public int BookingId { get; set; }
        public int Id { get; set; }

        public string Status { get; set; }
        public string? ContentPay { get; set; }
        public string? ContentPayAll { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PayOk { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
