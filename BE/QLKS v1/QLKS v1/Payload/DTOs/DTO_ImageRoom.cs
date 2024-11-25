using QLKS_v1.Entities;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_ImageRoom
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
       
        public string UrlImage { get; set; }
    }
}
