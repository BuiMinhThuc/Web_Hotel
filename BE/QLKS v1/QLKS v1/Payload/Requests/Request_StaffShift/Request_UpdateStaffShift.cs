using QLKS_v1.Entities;

namespace QLKS_v1.Payload.Requests.Request_StaffShift
{
    public class Request_UpdateStaffShift
    {
        public int Id { get; set; }
        public string User { get; set; }

        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string? ShiftTypes { get; set; }
    }
}
