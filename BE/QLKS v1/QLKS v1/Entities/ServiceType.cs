namespace QLKS_v1.Entities
{
    public class ServiceType:BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Service>? Services { get; set; }  
    }
}
