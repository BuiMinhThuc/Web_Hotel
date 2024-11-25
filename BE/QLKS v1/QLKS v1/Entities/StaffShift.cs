namespace QLKS_v1.Entities
{
    
    public class StaffShift:BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; } 
        public TimeSpan? TotalHoursWorked
        {
            get
            {
                return EndTime - StartTime;
            }
        } 
        public string? ShiftTypes { get; set; }
    }
  
}
