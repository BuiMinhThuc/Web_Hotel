using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_StaffShift
    {
        private readonly AppDbContext dbContext;

        public Converter_StaffShift(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DTO_StaffShift DTO_StaffShiftFull (StaffShift staffShift)
        {
            return new DTO_StaffShift
            {
                Id = staffShift.Id,
                UserName= dbContext.users.FirstOrDefault(x=>x.Id==staffShift.UserId).FullName,
                NumberPhone= dbContext.users.FirstOrDefault(x=>x.Id==staffShift.UserId).NumberPhone,
                StartTime = staffShift.StartTime,
                EndTime = staffShift.EndTime,
                Date= staffShift.Date,
                ShiftTypes = staffShift.ShiftTypes,


            };
        }
    }
}
