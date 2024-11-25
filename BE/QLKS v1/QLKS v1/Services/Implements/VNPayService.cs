using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QLKS_v1.ConfigModels.VnPayPayment;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Handle;
using QLKS_v1.Interfaces;
using QLKS_v1.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using QRCoder;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QLKS_v1.Payload.Requests.Request_BookingRoom;
using QLKS_v1.Payload.DTOs; // Để làm việc với MemoryStream



namespace QLKS_v1.Implements
{
    public class VNPayService : IVNPayService
    {

        private readonly IConfiguration _configuration;
        private readonly IService_User _authService;
        private readonly AppDbContext dbContext;

        public VNPayService()
        {
        }

        public VNPayService(IConfiguration configuration, IService_User authService, AppDbContext dbContext)
        {
            _configuration = configuration;
            _authService = authService;
            this.dbContext = dbContext;
        }

        public async Task<string> TaoDuongDanThanhToan(int BillId, HttpContext httpContext, int Customerid)
        {
            var hoaDon = await dbContext.bills.SingleOrDefaultAsync(x => x.Id == BillId);
            if (hoaDon == null) return "Hóa đơn không tồn tại.";
            if (hoaDon.Status) return "Hóa đơn đã được thanh toán trước đó.";
            if (hoaDon.TotalPrice == null || hoaDon.TotalPrice <= 0) return "Vui lòng kiểm tra lại hóa đơn.";

            VnPayLibrary vnpay = new VnPayLibrary();

            double vnp_Amount = (double)(hoaDon.TotalPrice * 100)*0.1; // Chuyển đổi giá trị TotalPrice

            // Thêm các tham số vào requestData
            vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", vnp_Amount.ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán hóa đơn: " + BillId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", BillId.ToString());

            string baseUrl = _configuration["VnPay:BaseUrl"];
            string hashSecret = _configuration["VnPay:HashSecret"];

            return vnpay.CreateRequestUrl(baseUrl, hashSecret);
        }



        public async Task<string> VNPayReturn(IQueryCollection vnpayData)
        {
            string vnp_TmnCode = _configuration.GetSection("VnPay:TmnCode").Value;
            string vnp_HashSecret = _configuration.GetSection("VnPay:HashSecret").Value;

            var vnPayLibrary = new VnPayLibrary();
            foreach (var (key, value) in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPayLibrary.AddResponseData(key, value);
                }
            }

            string hoaDonId = vnPayLibrary.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = vnPayLibrary.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnPayLibrary.GetResponseData("vnp_TransactionStatus");
            string vnp_SecureHash = vnPayLibrary.GetResponseData("vnp_SecureHash");
            double vnp_Amount = Convert.ToDouble(vnPayLibrary.GetResponseData("vnp_Amount"));
            bool check = vnPayLibrary.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            if (check)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    var hoaDon = await dbContext.bills.FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(hoaDonId));
                    if (hoaDon == null)
                    {
                        return "Không tìm thấy hóa đơn";
                    }

                    hoaDon.Status = true;
                    hoaDon.CreateTime = DateTime.Now;
                    var booking =await dbContext.bookings.FirstOrDefaultAsync(x => x.Id == hoaDon.BookingId);
                    List<DTO_BillFullRoom> roomAndQuantity = await dbContext.roomBookingBills
     .Include(x => x.RoomNameBills)  // Bao gồm RoomNameBills
     .AsNoTracking()
     .Where(x => x.BookingId == hoaDon.BookingId)
     .SelectMany(x => x.RoomNameBills.Select(rnb => new DTO_BillFullRoom
     {
         NameRoom = rnb.NameRoom,   // Lấy tên phòng
         Quantity = x.Quantity      // Lấy số lượng từ roomBookingBill
     }))
     .ToListAsync();

                    // Sử dụng vòng lặp foreach để xử lý bất đồng bộ
                    foreach (var item in roomAndQuantity) // Dùng ToList để tránh thay đổi danh sách trong vòng lặp
                    {
                        var roomtype = await dbContext.roomTypes.FirstOrDefaultAsync(y => y.RoomTypeName == item.NameRoomType);

                        if (roomtype != null)
                        {
                            // Giả sử bạn muốn cập nhật thuộc tính RoomTypeName của DTO_BillFullRoom
                            item.NameRoomType = roomtype.RoomTypeName;  // Cập nhật RoomTypeName từ roomtype
                        }
                    }

                    int? NumAdult = await dbContext.roomBookingBills
     .Include(x => x.RoomType)
     .AsNoTracking()
     .Where(x => x.BookingId == hoaDon.BookingId)
     .SumAsync(x => x.Quantity * x.RoomType.QuantityAdult);





                    var customer =await dbContext.customers.Include(c => c.Bookings).AsNoTracking()
                              .Where(c => c.Bookings.Any(b => b.Id == hoaDon.BookingId))
                              .FirstOrDefaultAsync();
                    var TypeRoomList = await dbContext.roomBookingBills
     .Include(x => x.RoomType)
     .AsNoTracking()
     .Where(x => x.BookingId == hoaDon.BookingId)
     .GroupBy(x => x.RoomType.RoomTypeName)
     .Select(g => new
     {
         RoomTypeName = g.Key,
         Quantity = g.Sum(x => x.Quantity)
     })
     .ToListAsync();
                    string contenPer = null;

                    foreach (var item in TypeRoomList)
                    {
                        if(contenPer == null)
                        {
                            contenPer += $"({item.Quantity }) {item.RoomTypeName}";
                        }else
                            contenPer += $" + ({item.Quantity}) {item.RoomTypeName}";
                    }


                    if (customer != null)
                    {
                        string email = customer.Email;
                        // Chuỗi qrContent với các thông tin cơ bản
                        string qrContent = $"Thanh toán đặt cọc thành công {vnp_Amount/100}VNĐ!\nTên khách hàng: {customer.FullName}\nHóa đơn: {hoaDon.Id}\nSố điện thoại: {customer.Phone}" +
                            $"\nNgày đặt phòng: {DateTime.Now}\nNgày nhận phòng: {booking.CheckInDate}\nNgày trả phòng: {booking.CheckOutDate}\n";

                        // Thêm thông tin về tên phòng, số lượng và loại phòng
                        qrContent += "Danh sách phòng đã đặt:\n";

                        foreach (var room in roomAndQuantity)
                        {
                            var tempt = await dbContext.rooms.FirstOrDefaultAsync(x => x.RoomNumber == room.NameRoom);
                            var tempt2 = await dbContext.roomTypes.FirstOrDefaultAsync(x => x.Id == tempt.RoomTypeID);
                            qrContent += $"- Tên phòng: {room.NameRoom}-{tempt2.RoomTypeName}-{(double)tempt2.PricePerNight}VNĐ\n";
                        }
                        qrContent += $"Số trẻ em:{booking.NumberOfChildren} (100,000 vnđ/1 trẻ em)  !\n";
                        qrContent += $"Thành tiền : {(double)hoaDon.TotalPrice}Vnđ!\n";
                        qrContent += "--------------------------------------------------------------\n";
                        qrContent += "Xin quý khách xem thêm thông tin phòng tại website !\n";

                        // Sử dụng API của qrserver để tạo URL mã QR
                        string qrCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={Uri.EscapeDataString(qrContent)}";

                        // Tạo nội dung email với URL mã QR
                        /*  EmailTo emailTo = new EmailTo
                          {
                              Mail = email,
                              Subject = $"FLORENTINO THÔNG BÁO: {hoaDon.Id}",
                              Content = $"Quý khách đã thanh toán thành công!<br/>Mã QR:<br/><img src='{qrCodeUrl}' alt='Mã QR Thanh Toán' />"
                          };*/
                        EmailTo emailTo = new EmailTo
                        {
                            Mail = email,
                            Subject = $"FLORENTINO THÔNG BÁO: {hoaDon.Id}",
                            Content = $@"
        
        <table style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif;'>
            <tr style='background-color: #f7b731; color: #ffffff;'>
                <th style='border: 1px solid #ddd; padding: 12px; text-align: left;'>Thông tin</th>
                <th style='border: 1px solid #ddd; padding: 12px; text-align: left;'>Chi tiết</th>
            </tr>
            <tr>
                <td style='border: 1px solid #ddd; padding: 8px; background-color: #fdf2e9;'>Người đại diện</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{customer.FullName} SDT: {customer.Phone}</td>
            </tr>
            <tr>
                <td style='border: 1px solid #ddd; padding: 8px; background-color: #fdf2e9;'>Số lượng khách</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{NumAdult} Người lớn + {booking.NumberOfChildren} Trẻ em</td>
            </tr>
            <tr>
                <td style='border: 1px solid #ddd; padding: 8px; background-color: #fdf2e9;'>Check-in</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>14h ngày {booking.CheckInDate.ToString("dd/MM/yyyy")}</td>
            </tr>
            <tr>
                <td style='border: 1px solid #ddd; padding: 8px; background-color: #fdf2e9;'>Check-out</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>12h ngày {booking.CheckOutDate.ToString("dd/MM/yyyy")}</td>
            </tr>
            <tr>
                <td style='border: 1px solid #ddd; padding: 8px; background-color: #fdf2e9;'>Hạng phòng</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{contenPer}</td>
            </tr>
        </table>
        <br/>
        <p>Quý khách đã thanh toán thành công!</p>
        <p>Mã QR:</p>
        <img src='{qrCodeUrl}' alt='Mã QR Thanh Toán' />"
                        };



                        // Cấu hình và gửi email
                        var smtpClient = new SmtpClient("smtp.gmail.com")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential("buiminhthucvv2002@gmail.com", "rhum rqpf hvvm pbca"),
                            EnableSsl = true
                        };
                        var message = new MailMessage
                        {
                            From = new MailAddress("buiminhthucvv2002@gmail.com"),
                            Subject = emailTo.Subject,
                            Body = emailTo.Content,
                            IsBodyHtml = true
                        };
                        message.To.Add(emailTo.Mail);
                        smtpClient.Send(message);
                    }
                    foreach (var room in roomAndQuantity)
                    {
                        var tempt = await dbContext.rooms.FirstOrDefaultAsync(x => x.RoomNumber == room.NameRoom);
                        var tempt2 = await dbContext.roomTypes.FirstOrDefaultAsync(x => x.Id == tempt.RoomTypeID);
                        hoaDon.ContentPay = $"Đặt cọc";
                       /* if (hoaDon.ContentPay == null)
                        {
                            hoaDon.ContentPay += $"{room.NameRoom}\n";
                        }else
                            hoaDon.ContentPay += $",{room.NameRoom}\n";*/

                        if(hoaDon.ContentPayAll==null)
                            hoaDon.ContentPayAll += $"{room.NameRoom}\n";
                        else
                        hoaDon.ContentPayAll += $",{room.NameRoom}\n";

                }
                    hoaDon.PayOk = (decimal)vnp_Amount/100;
                    var customer2 = await dbContext.customers.Include(c => c.Bookings)
                              .Where(c => c.Bookings.Any(b => b.Id == hoaDon.BookingId))
                              .FirstOrDefaultAsync();
                    customer2.ContentPay = hoaDon.ContentPayAll;

                    //dbContext.customers.Update(customer);
                    dbContext.bills.Update(hoaDon);
                   await dbContext.SaveChangesAsync();

                    dbContext.customers.Update(customer2);
                    await dbContext.SaveChangesAsync();
                    return "http://127.0.0.1:5501/FE/Member/HTML/Home.html?checkPay=True";
                }
                else
                {
                    var hoaDon = dbContext.bills.FirstOrDefault(x => x.Id == Convert.ToInt32(hoaDonId));
                    var booking = dbContext.bookings.FirstOrDefault(x => x.Id == hoaDon.BookingId);
                    dbContext.bookings.Remove(booking);
                    dbContext.SaveChanges();
                    return "http://127.0.0.1:5501/FE/Member/HTML/Home.html";
                }
            }
            else
            {
                return "Có lỗi trong quá trình xử lý";
            }
        }
    }
}
