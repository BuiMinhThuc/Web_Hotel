using QLKS_v1.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_RoomHot
    {
        public int id {  get; set; }
        public string RoomNumber { get; set; }

        public string RoomTypeName { get; set; }
        public List<string> RoomImages { get; set; }
    }
}
