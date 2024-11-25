namespace QLKS_v1.Payload.Requests.Request_Promotion
{
    public class Request_UpdatePromotion
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public double PriceOff { get; set; }
    }
}
