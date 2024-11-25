using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_StaffShift;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;
using System.Text.RegularExpressions;

namespace QLKS_v1.Services.Implements
{
    public class Service_StaffShift : IService_StaffShift
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_StaffShift converter_StaffShift;
        private readonly ResponseBase response;

        public Service_StaffShift(AppDbContext dbContext, Converter_StaffShift converter_StaffShift, ResponseBase response)
        {
            this.dbContext = dbContext;
            this.converter_StaffShift = converter_StaffShift;
            this.response = response;
        }

        public async Task<ResponseBase> CreateStaffShift(Request_CreateStaffShift request)
        {
            Match match = Regex.Match(request.User, @"^\d+");
            int UserId = int.Parse(match.Value);

            var staff = await dbContext.users.FirstOrDefaultAsync(x => x.Id == UserId);
            var sts = new StaffShift();
            sts.StartTime = request.StartTime; ;
            sts.EndTime = request.EndTime; 
            sts.ShiftTypes = request.ShiftTypes; 
            sts.UserId = staff.Id;
            sts.Date=request.Date;
            dbContext.staffShifts.Add(sts);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Thêm lịch thành công !");
        }

        public async Task<ResponseBase> DeleteStaffShift(int request)
        {
            var sts = await dbContext.staffShifts.FirstOrDefaultAsync(x => x.Id == request);
            dbContext.staffShifts.Remove(sts);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Xóa lịch thành công !");
        }

        public IQueryable<DTO_StaffShift> GetListStaffShift(int pageSize, int pageNumber)
        {
            return dbContext.staffShifts.AsQueryable().AsNoTracking().Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x=>converter_StaffShift.DTO_StaffShiftFull(x));
        }

        public async Task<ResponseBase> UpdateStaffShift(Request_UpdateStaffShift request)
        {
            Match match = Regex.Match(request.User, @"^\d+");
            int UserId = int.Parse(match.Value);

            var staff = await dbContext.users.FirstOrDefaultAsync(x => x.Id == UserId);
            var sts = await dbContext.staffShifts.FirstOrDefaultAsync(x => x.Id==request.Id);
            sts.StartTime = request.StartTime; ;
            sts.EndTime = request.EndTime;
            sts.ShiftTypes = request.ShiftTypes;
            sts.UserId = staff.Id;
            sts.Date = request.Date;
            dbContext.staffShifts.Update(sts);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Sửa lịch thành công !");
        }
    }
}
