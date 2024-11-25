using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLKS_v1.Entities
{
    public class Room: BaseEntity
    {
        public string RoomNumber { get; set; }

        public int RoomTypeID { get; set; }

        

        [ForeignKey("RoomTypeID")]
        public RoomType? RoomType { get; set; }

        
        public ICollection<Equipment>? Equipments { get; set; }
        public ICollection<RoomImage>? RoomImages { get; set; }
 
    }
}
