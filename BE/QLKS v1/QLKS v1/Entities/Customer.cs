using System.ComponentModel.DataAnnotations;

namespace QLKS_v1.Entities
{
    public class Customer:BaseEntity
    {
       
        public string FullName { get; set; }

        
        public string Phone { get; set; }

     
        public string? Email { get; set; }
        public string? CardNumber { get; set; }
        public string? ContentPay { get; set; }
        public DateTime? CreateBooking { get; set; }

    
        public string? Address { get; set; }


        public string? Nationality { get; set; }

   
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
       
        
    }
}
