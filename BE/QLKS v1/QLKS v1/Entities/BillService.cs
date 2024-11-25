using System.ComponentModel.DataAnnotations.Schema;

namespace QLKS_v1.Entities
{
    public class BillService:BaseEntity
    {
        public int BillId {  get; set; }
        public int? UserId {  get; set; }
        public User? User {  get; set; }
        public Bill? Bill { get; set; }

        public int Quantity {  get; set; }
        public string? RoomName {  get; set; }

        public int ServiceId { get; set; }
        public bool? Status { get; set; } = false;
        public DateTime? CreateTime { get; set; }=DateTime.Now;
        public Service? Service { get; set; }

    }
}
