namespace QLKS_v1.Payload.Requests.Request_News
{
    public class Request_UpdateNews
    {
        public int Id { get; set; }
        public string Title {  get; set; }
        public string Content {  get; set; }
        public IFormFile UrlImg {  get; set; }

    }
}
