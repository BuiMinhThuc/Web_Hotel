namespace QLKS_v1.Entities
{
    public class Service:BaseEntity
    {
        public string Name {  get; set; }
        public int ServiceTypeId {  get; set; }
        public ServiceType? ServiceType { get; set; }

        public decimal Price { get; set; }
        
        public ICollection<BillService>? BillServices { get; set; }
    }
}
