namespace QLKS_v1.Payload.Requests.Request_CustomerFeedBack
{
    public class Request_CreateFeedBack
    {
        public int UserId { get; set; }

        public string  Content { get; set; }
        public int  Rate { get; set; }
    }
}
