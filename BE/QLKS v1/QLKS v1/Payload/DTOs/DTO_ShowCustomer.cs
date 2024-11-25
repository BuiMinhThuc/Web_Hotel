namespace QLKS_v1.Payload.DTOs
{
    public class DTO_ShowCustomer
    {
        public int Id { get; set; }
        public string FullName { get; set; }


        public string Phone { get; set; }


        public string? Email { get; set; }
        public string? CardNumber { get; set; }


        public string? Nationality { get; set; }


        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
