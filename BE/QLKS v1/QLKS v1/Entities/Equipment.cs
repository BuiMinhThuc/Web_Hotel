namespace QLKS_v1.Entities
{
    public class Equipment:BaseEntity
    {
        public int RoomId { set; get; }
        public Room? Room { set; get; }
        public string Name { set; get; }
    }
}
