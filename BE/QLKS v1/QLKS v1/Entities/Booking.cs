using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QLKS_v1.Entities
{
    public class Booking : BaseEntity
    {

        public int CustomerID { get; set; }
    

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public int? NumberOfChildren { get; set; }

        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; }
 

        public ICollection<Bill>? Bills { get; set; }
        public ICollection<RoomBookingBill>? RoomBookingBillS { get; set; }
       
    }

   
}
