using QLKS_v1.Entities;

namespace QLKS_v1.Payload.DTOs
{
    public class DTO_StaffShift
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string NumberPhone { get; set; }
       
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }     
        public string? ShiftTypes { get; set; }
    }
}
