using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_BookingRoom;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_Booking
    {
        private readonly AppDbContext dbContext;

        public Converter_Booking(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DTO_Booking EntityToDTO(Booking booking)
        {
            var bill = dbContext.bills.FirstOrDefault(x => x.BookingId == booking.Id);
            var customer = dbContext.customers.FirstOrDefault(x => x.Id == booking.CustomerID);
            var roomBookingBills = dbContext.roomBookingBills.Where(x => x.BookingId == booking.Id).ToList();

            // Khởi tạo listRoomName dựa trên roomBookingBills
            var listRoomName = roomBookingBills
                .SelectMany(x => dbContext.roomNameBills.Where(y => y.RoomBookingBillId == x.Id).Select(y => y.NameRoom))
                .ToList();

            return new DTO_Booking
            {
                ID = booking.Id,
                billId = bill?.Id ?? 0, // Xử lý null cho bill
                CustomerID = booking.CustomerID,
                CardNumber=customer.CardNumber?? "Chưa CheckIn",
                CustomerName = customer?.FullName ?? "Unknown", // Xử lý null cho customer
                NumberPhone = customer?.Phone ?? "Unknown", // Xử lý null cho số điện thoại
                listRoomType = roomBookingBills
                    .Select(x => new Requesr_ListRoom
                    {
                        RoomTypeName = dbContext.roomTypes.FirstOrDefault(y => y.Id == x.RoomTypeId)?.RoomTypeName ?? "Unknown", // Xử lý null cho roomType
                        Quantity = x.Quantity
                    }).ToList(),
                listRoomName = listRoomName,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                NumberOfChild = booking.NumberOfChildren,
                TotalPrice= bill.TotalPrice
            };
        }


    }
}
