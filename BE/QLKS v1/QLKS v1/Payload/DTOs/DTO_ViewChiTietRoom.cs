using QLKS_v1.Entities;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_ViewChiTietRoom
    {
        public string RoomName {  get; set; }
        public string RoomTypeName {  get; set; }
        public decimal Price {  get; set; }
        public List<string> Equipments { get; set; }
    }
}
