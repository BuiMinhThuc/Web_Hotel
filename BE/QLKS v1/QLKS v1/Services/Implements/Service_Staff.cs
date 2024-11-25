using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Handle;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Staff;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_Staff : IService_Staff
    {
        private readonly AppDbContext dbContext;

        private readonly Converter_Staff converterStaff;

        private readonly ResponseBase responseBase;

        public Service_Staff(AppDbContext dbContext, Converter_Staff converterStaff, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.converterStaff = converterStaff;
            this.responseBase = responseBase;
        }

        public async Task<ResponseBase> DeleteStaff(int Id )
        {
            var staff = await dbContext.users.FirstOrDefaultAsync(x => x.Id == Id);
           
            dbContext.users.Remove(staff);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase($"Xóa nhân viên Id:{staff.Id} thành công !");
        }

        public async  Task<IQueryable<DTO_Staff>> getListStaff(int pageSize,int pageNumber)
        {
            return await Task.FromResult(dbContext.users.OrderByDescending(x=>x.Id).Include(x=>x.Role)
                .AsNoTracking()
                .Where(x=>x.Role.Code=="Staff")
                .Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x=>converterStaff.EntityToDTO(x)));
        }

        public async Task<ResponseBase> UpdateStaff(Request_UpdateStaff request)
        {

            if (!CheckInput.IsValiEmail(request.Email))
            {
               return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest,"Email không hợp lệ !");
            }
            if (CheckInput.IsValidPhoneNumber(request.PhoneNumber)!=request.PhoneNumber)
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Số điện thoại không hợp lệ !");
            }



            var staff = await dbContext.users.FirstOrDefaultAsync(x=>x.Id==request.Id);
            staff.FullName = request.Name;
            staff.NumberPhone = request.PhoneNumber;
            staff.Email=request.Email;
            staff.Address= request.Address;
            staff.Gender = request.Gender;
            dbContext.users.Update(staff);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase($"Cập nhật nhân viên Id:{staff.Id} thành công !");
        }
    }
}
