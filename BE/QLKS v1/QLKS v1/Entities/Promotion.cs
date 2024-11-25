namespace QLKS_v1.Entities
{
    public class Promotion:BaseEntity
    {
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }

        public double PriceOff {  get; set; }
    }
}
