namespace QLKS_v1.Entities
{
    public class CustomerFeedback:BaseEntity
    {
      
        public int? UserId {  get; set; }
        public string Content {  get; set; }
        public int rate { get; set; }
    
         public User? User { get; set; }
    }
}
