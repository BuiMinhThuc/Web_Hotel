using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Handle;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_BookingService;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QLKS_v1.Services.Implements
{
    public class Service_BookingService : IService_BookingService
    {
        private readonly AppDbContext dbContext;
        private readonly ResponseBase response;
        private readonly Converter_BookingService converter;

        public Service_BookingService(AppDbContext dbContext, ResponseBase response, Converter_BookingService converter)
        {
            this.dbContext = dbContext;
            this.response = response;
            this.converter = converter;
        }

        public bool IsBookingDateConflicted(DateTime date, string roomNumber)
        {
            // Truy vấn kiểm tra xem có bất kỳ đặt phòng nào có khoảng thời gian xung đột với ngày đến và ngày đi đã chọn
            var isConflicted = dbContext.bookings
                .Include(x => x.RoomBookingBillS)
                .ThenInclude(x => x.RoomNameBills)
                .AsNoTracking() // Không cần theo dõi thay đổi, chỉ đọc dữ liệu
                .Any(booking =>
                    booking.RoomBookingBillS
                        .Any(rbb => rbb.RoomNameBills
                            .Any(rnb => rnb.NameRoom == roomNumber) // Kiểm tra xem roomNumber có trùng không
                        ) &&
                    (date >= booking.CheckInDate && date <= booking.CheckOutDate)); // Kiểm tra xem ngày có xung đột không

            return isConflicted; // Trả về true nếu có xung đột, ngược lại trả về false
        }
        

        public async Task<ResponseBase> CreateBookingService(Request_CreateBookingService request)
        {
            if (request.Quantity == null)
            {
                return response.ResponseErrorBase(StatusCodes.Status400BadRequest, "Vui lòng nhập số lượng !");
            }
            string roomName = request.RoomName.Trim();
            int lastSpaceIndex = roomName.LastIndexOf('-');
            string roomNumber = roomName.Substring(lastSpaceIndex + 1).Trim();
            Console.WriteLine(roomNumber);
            DateTime dateToday = DateTime.Now;
            if (!IsBookingDateConflicted(dateToday, roomNumber))
            {
                return response.ResponseErrorBase(StatusCodes.Status400BadRequest, "Phòng đang không được sử dụng !");
            }


            var booking = await dbContext.bookings
             .Include(b => b.RoomBookingBillS)
                .ThenInclude(rbb => rbb.RoomNameBills)
                 .AsNoTracking()
                 .Where(b => b.RoomBookingBillS
                     .Any(rbb => rbb.RoomNameBills
                       .Any(rnb => rnb.NameRoom == roomNumber)) && // Kiểm tra roomNumber trong RoomNameBills
               DateTime.Now >= b.CheckInDate && DateTime.Now <= b.CheckOutDate) // Kiểm tra thời gian
                      .FirstOrDefaultAsync();

            var bill = await dbContext.bills.Include(x => x.Booking).AsNoTracking()
                .Where(x => x.BookingId == booking.Id).FirstOrDefaultAsync();
            string ServiceNameandPrice = request.ServiceName.Trim();
            int lastSpaceIndex1 = ServiceNameandPrice.LastIndexOf(' ');
            string ServiceName = ServiceNameandPrice.Substring(0, lastSpaceIndex1).Trim();
            var service = await dbContext.services.FirstOrDefaultAsync(x => x.Name == ServiceName);
            var BookingService = new BillService
            {
                BillId = bill.Id,
                Quantity = request.Quantity,
                ServiceId = service.Id,
                RoomName= roomNumber,
                UserId=request.UserId is null? 14: request.UserId,
                
            };
            dbContext.billServices.Add(BookingService);
            await dbContext.SaveChangesAsync();

           /* bill.TotalPrice += (service.Price * request.Quantity);*/
          
           /*  bill.ContentPayAll = BillHelper.UpdateString(bill.ContentPayAll, service.Name);*/
            dbContext.bills.Update(bill);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Đặt dịch vụ thành công !");
        }

        public async Task<ResponseBase> DeleteBookingService(int request)
        {
            var billsr= await dbContext.billServices.FirstOrDefaultAsync(x => x.Id == request);
            if (billsr.Status == true)
            {
                return response.ResponseErrorBase(StatusCodes.Status400BadRequest, "Đơn đã hoàn thành rồi, không thể hủy !");
            }
            dbContext.billServices.Remove(billsr);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Hủy dịch vụ thành công !");
        }

        

        public async Task<ResponseBase> UpdateBookingService(int billServiceId)
        {
            var billService = await dbContext.billServices.FirstOrDefaultAsync(x => x.Id == billServiceId);
            var service = await dbContext.services.FirstOrDefaultAsync(x => x.Id == billService.ServiceId);

            billService.Status = true;
            var bill = await dbContext.bills.FirstOrDefaultAsync(x => x.Id == billService.BillId);
            bill.TotalPrice += service.Price*billService.Quantity;
            bill.ContentPayAll = BillHelper.UpdateString(bill.ContentPay, service.Name);
            dbContext.bills.Update(bill);
            await dbContext.SaveChangesAsync();
            dbContext.billServices.Update(billService); 
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Đơn đã hoàn thành !");



        }

        public  IQueryable<DTO_BookingService> GetListBookingService(int pageSize, int pageNumber)
        {
            return dbContext.billServices.OrderByDescending(x => x.Id).Skip((pageNumber-1)*pageSize).Take(pageSize).AsQueryable().Select(x=>converter.EntityToDTO(x));
        }

        public IQueryable<DTO_BookingService> GetListBookingServiceForCase(string Type, int pageSize, int pageNumber)
        {
            if(Type== "All")
                return dbContext.billServices.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsQueryable().Select(x => converter.EntityToDTO(x));
            else if(Type== "NotOk")
                return dbContext.billServices.OrderByDescending(x => x.Id).Where(x=>x.Status==false).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsQueryable().Select(x => converter.EntityToDTO(x));
            else
                return dbContext.billServices.OrderByDescending(x => x.Id).Where(x => x.Status == true).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsQueryable().Select(x => converter.EntityToDTO(x));
        }

        public async Task<IQueryable<DTO_BookingService>> GetListBookingServiceForUser(int UserId, int pageSize, int pageNumber)
        {
           return await Task.FromResult(dbContext.billServices
               .AsNoTracking()
               .Where(x=>x.UserId==UserId)
               .Skip((pageNumber - 1)*pageSize)
               .Take(pageSize)
               .Select(x=>converter.EntityToDTO(x)));
        }
    }
}
