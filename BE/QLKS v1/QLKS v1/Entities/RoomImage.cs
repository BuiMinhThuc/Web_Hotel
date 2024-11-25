namespace QLKS_v1.Entities
{
    public class RoomImage:BaseEntity
    {
        public int RoomId {  get; set; }
        public Room? Room { get; set; }
        public string UrlImage {  get; set; }
    }
}
