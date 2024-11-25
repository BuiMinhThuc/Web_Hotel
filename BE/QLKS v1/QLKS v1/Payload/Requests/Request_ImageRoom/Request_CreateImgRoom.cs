namespace QLKS_v1.Payload.Requests.Request_ImageRoom
{
    public class Request_CreateImgRoom
    {
        public int RoomId { get; set; }
        public IFormFile UrlImg {  get; set; }
    }
}
