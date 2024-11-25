namespace QLKS_v1.Entities
{
    public class ComfirmEmail : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime RequiredDateTime { get; set; }= DateTime.Now;
        public DateTime ExpiredDateTime { get; set; }=DateTime.Now.AddMinutes(5);
        public string ConfirmCode { get; set; }
        public bool IsConfirm { get; set; } = false;
    }
}
