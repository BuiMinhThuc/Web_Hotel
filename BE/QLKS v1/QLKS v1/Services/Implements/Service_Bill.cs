using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Signatures;
using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;


namespace QLKS_v1.Services.Implements
{
    public class Service_Bill : IService_Bill
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_Bill converter_Bill;
        private readonly ResponseBase responseBase;

        public Service_Bill(AppDbContext dbContext, Converter_Bill converter_Bill, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.converter_Bill = converter_Bill;
            this.responseBase = responseBase;
        }

        public async Task<string> createBillPdf(int billId)
        {
            var bill = await dbContext.bills.FindAsync(billId);
            var booking = await dbContext.bookings.FindAsync(bill.BookingId);
            var customer = await dbContext.customers.FindAsync(booking.CustomerID);


            string content = "Khách sạn HOTELS\n";
            content += "Địa chỉ: Ngõ 92 Bắc Từ Kiêm Hà Nội\n";
            content += "------------------------------------------";
            content += "Hóa đơn thanh toán\n\n";
            content += $"Khách hàng: {customer.FullName}\n";
            return content;

        }

        public IQueryable<DTO_Bill> GetAll(int pageSize,int pageNumber)
        {
            return dbContext.bills.OrderByDescending(x => x.Id).Skip((pageNumber-1)*pageSize).Take(pageSize).AsQueryable().Select(x=> converter_Bill.EntityDTO(x));
        }

        public async Task<DTO_BillToPDF> PrintBill(int billId)
        {
            var bill = await dbContext.bills.FirstOrDefaultAsync(x=>x.Id==billId);

            return converter_Bill.DTO_BillToPDFToDTO(bill);
        }

        public async Task<int> TotalPersonAdult()
        {


            var currentDate = DateTime.UtcNow.Date; // Sử dụng UTC để tránh vấn đề với múi giờ hoặc sử dụng .Date nếu bạn chỉ cần phần ngày

            var adultCount = dbContext.bookings
                .Where(b => b.CheckInDate.Date <= currentDate && b.CheckOutDate.Date >= currentDate) // So sánh chỉ phần ngày
                .Where(b => b.Customer != null && !string.IsNullOrEmpty(b.Customer.CardNumber)) // Đảm bảo khách hàng có thẻ căn cước
                .SelectMany(b => b.RoomBookingBillS)
                .Sum(rb => rb.Quantity * (rb.RoomType.QuantityAdult ?? 0)); // 

            return adultCount;



            /* int? NumAdult = await dbContext.roomBookingBills
                 .Include(x => x.RoomType)

                 .AsNoTracking()
                 .Where(x => x.Booking.CheckInDate <= DateTime.Now
                             && x.Booking.CheckOutDate >= DateTime.Now
                             && x.Booking.Customer.CardNumber == null)
                 .SumAsync(x => (x.Quantity ) * (x.RoomType.QuantityAdult ?? 0));

             return NumAdult ?? 0;*/
        }


        public async Task<int> TotalPersonChild()
        {
            return await dbContext.bookings.AsNoTracking()
                .Where(x=> x.Customer.CardNumber != null && x.CheckInDate.Date <= DateTime.UtcNow.Date && x.CheckOutDate.Date >= DateTime.Now.Date)
                .SumAsync(x => (int?)x.NumberOfChildren ?? 0);
        }

        public async Task<ResponseBase> UpdateBill(int billId,int UserId)
        {
            var bill = await dbContext.bills.FirstOrDefaultAsync(x => x.Id == billId);
            var booking = await dbContext.bookings.FirstOrDefaultAsync(x => x.Id == bill.BookingId);
            var customer = await dbContext.customers.FirstOrDefaultAsync(x => x.Id == booking.CustomerID);
            if (bill == null)
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest,"Hóa đơn không tồn tại !");
            var ListBillService = await dbContext.billServices.Include(x=>x.Bill).Where(x=>x.Bill.Id==billId).ToListAsync();

            foreach(var item  in ListBillService)
            {
                if(item.Status ==false)
                {
                    var service = await dbContext.services.FirstOrDefaultAsync(x => x.Id == item.ServiceId);
                    return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, $"Dịch vụ có ID: {item.Id} là {service.Name} phòng {item.RoomName} chưa hoàn thành ! ");
                }
            }


            bill.UserId= UserId;
            bill.PayOk = bill.TotalPrice;
            bill.ContentPay =bill.ContentPayAll;
            customer.ContentPay = bill.ContentPayAll;
            dbContext.bills.Update(bill);
            dbContext.customers.Update(customer);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase( " Thanh toán Checkout thành công !");

        }
    }
}
