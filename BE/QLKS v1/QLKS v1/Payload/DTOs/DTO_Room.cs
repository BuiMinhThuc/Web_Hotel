using QLKS_v1.DataContext;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_Room
    {
        public int Id { get; set; }
       public string RoomName { get; set; }
        public string RoomType {  get; set; }
        public decimal Price {  get; set; }



    }
}
