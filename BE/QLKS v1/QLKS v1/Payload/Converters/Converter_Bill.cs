using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_BookingRoom;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_Bill
    {
        private readonly AppDbContext appDbContext;

        public Converter_Bill(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public DTO_Bill EntityDTO(Bill bill)
        {
            return new DTO_Bill
            {
                Id = bill.Id,
                BookingId = bill.BookingId,
                TotalPrice = bill.TotalPrice,
                Status = bill.Status == true ? "Đã thanh toán" : "Chưa thanh toán",
                CreateTime = bill.CreateTime,
                PayOk = bill.PayOk,
                ContentPay = bill.ContentPay,
                ContentPayAll = bill.ContentPayAll,
            };
        }

        public DTO_BillToPDF DTO_BillToPDFToDTO(Bill bill)
        {
            var booking = appDbContext.bookings.FirstOrDefault(x => x.Id == bill.BookingId);
            var customer = appDbContext.customers.FirstOrDefault(x => x.Id == booking.CustomerID);
            var User = appDbContext.users.FirstOrDefault(x => x.Id == bill.UserId);

            return new DTO_BillToPDF
            {
                Id = bill.Id,
                UserId=User.Id,
                NameStaff=User.FullName,
                NameCustomer = customer.FullName, // Trả về tên khách hàng đầu tiên tìm thấy
                NumberPhone = customer.Phone, // Trả về tên khách hàng đầu tiên tìm thấy
                ListTypeRoom = appDbContext.roomBookingBills
                .Where(x => x.BookingId == booking.Id).Select(x => new Requests.Request_BookingRoom.Request_ListRoomType
                {
                    RoomTypeId = x.RoomTypeId,
                    RoomTypeName = appDbContext.roomTypes.FirstOrDefault(y=>y.Id==x.RoomTypeId).RoomTypeName,
                    Quantity = x.Quantity,
                    PriceOfOne= appDbContext.roomTypes.FirstOrDefault(z=>z.RoomTypeName== appDbContext.roomTypes.FirstOrDefault(y => y.Id == x.RoomTypeId).RoomTypeName).PricePerNight
                }).ToList(),
                child=booking.NumberOfChildren,
                ListRoomName =appDbContext.roomNameBills
                                    .Include(x=>x.RoomBookingBill)
                                        .ThenInclude(x=>x.Booking)
                                           .AsNoTracking()
                                            .Where(x=>x.RoomBookingBill.Booking.Id==booking.Id)
                                                .Select(x=>new Request_ListRoomNamePDF
                                                {
                                                    RoomTypeId=appDbContext.rooms.FirstOrDefault(Y=>Y.RoomNumber==x.NameRoom).RoomTypeID,
                                                    Name= appDbContext.rooms.FirstOrDefault(Y => Y.RoomNumber == x.NameRoom).RoomNumber
                                                }).ToList(),
                CheckInDate = booking.CheckInDate,
                CheckOutDate= booking.CheckOutDate,
                ServiceBill = appDbContext.billServices
    .Where(x => x.BillId == bill.Id)
    .GroupBy(x => appDbContext.services.FirstOrDefault(y => y.Id == x.ServiceId).Name)
    .Select(group => new DTO_BillServiceForPDF
    {
        ServiceName = group.Key,
        Quantity = group.Sum(x => x.Quantity),
        PriceOfOne= appDbContext.services.FirstOrDefault(x=>x.Name==group.Key).Price,
    })
    .ToList() ?? null,

            TotalPrice = bill.TotalPrice

            };
        } 



        
    } 
    
}
