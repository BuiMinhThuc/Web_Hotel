using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Handle;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Booking;
using QLKS_v1.Payload.Requests.Request_BookingRoom;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;

namespace QLKS_v1.Services.Implements
{
    public class Service_Booking : IService_Booking
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_Booking converter;
        private readonly ResponseBase responseBase;
        private readonly ResponseObject<DTO_Booking> response;
        private readonly IConfiguration configuration;

        public Service_Booking(AppDbContext dbContext, Converter_Booking converter, ResponseBase responseBase, ResponseObject<DTO_Booking> response, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.converter = converter;
            this.responseBase = responseBase;
            this.response = response;
            this.configuration = configuration;
        }



        private List<Room> GetAvailableRooms(List<Requesr_ListRoom> roomTypeList, DateTime checkInDate, DateTime checkOutDate)
        {
            List<Room> availableRooms = new List<Room>();

            foreach (var roomType in roomTypeList)
            {
                var roomTypeId = dbContext.roomTypes
                                           .Where(rt => rt.RoomTypeName == roomType.RoomTypeName)
                                           .Select(rt => rt.Id)
                                           .FirstOrDefault();

                if (roomTypeId > 0) // Kiểm tra xem loại phòng có tồn tại không
                {
                    // Lấy danh sách các phòng trống cho loại phòng hiện tại
                    var rooms = GetAvailableRoomList(roomTypeId, checkInDate, checkOutDate);

                    // Chỉ lấy đủ số lượng yêu cầu
                    var roomsToAdd = rooms.Take(roomType.Quantity).ToList(); // Lấy số phòng tối đa theo yêu cầu
                    availableRooms.AddRange(roomsToAdd);
                }
            }

            return availableRooms; // Trả về danh sách các phòng đủ số lượng yêu cầu
        }



        private List<Room> GetAvailableRoomList(int roomTypeId, DateTime checkInDate, DateTime checkOutDate)
        {
            // Lấy danh sách các RoomID đã được đặt trong khoảng thời gian yêu cầu
            var bookedRoomNumbers = dbContext.bookings
                                             .Where(b => b.CheckInDate < checkOutDate && b.CheckOutDate > checkInDate)
                                             .SelectMany(b => b.RoomBookingBillS)
                                             .Where(rbb => rbb.RoomTypeId == roomTypeId)
                                             .SelectMany(rbb => rbb.RoomNameBills)
                                             .Select(rnb => rnb.NameRoom)
                                             .Distinct()
                                             .ToList();

            // Lấy tất cả các Room liên quan đến RoomTypeID này và không nằm trong danh sách đã đặt
            var availableRooms = dbContext.rooms
                                          .Where(r => r.RoomTypeID == roomTypeId && !bookedRoomNumbers.Contains(r.RoomNumber))
                                          .ToList(); // Lấy tất cả phòng không bị trùng lặp

            return availableRooms; // Trả về danh sách phòng trống
        }








        public async Task<ResponseObject<DTO_Booking>> CreateBooking(Request_CreateBooking request)
        {
            bool check1 = true;
            foreach (var item in request.RoomTypeList)
            {
                if (item.Quantity != 0)
                {
                    check1 = false;
                    break;
                }
            }
            if (check1)
            {
                return response.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng chọn phòng !", null);
            }

            // Kiểm tra và xử lý các trường hợp dữ liệu không hợp lệ
            DateTime ngayden=request.CheckInDate, ngaydi=request.CheckOutDate;
            

            if (ngayden < DateTime.UtcNow.AddDays(-1))
            {
                return response.ResponseError(StatusCodes.Status400BadRequest, "Ngày đến không hợp lệ!", null);
            }

            if (string.IsNullOrWhiteSpace(request.FullName) || request.NumberPhone == null)
            {
                return response.ResponseError(StatusCodes.Status400BadRequest, "Tên hoặc số điện thoại không hợp lệ!", null);
            }

            if (!CheckInput.IsValiEmail(request.Email) || request.NumberPhone != CheckInput.IsValidPhoneNumber(request.NumberPhone))
            {
                return response.ResponseError(StatusCodes.Status400BadRequest, "Email hoặc số điện thoại không hợp lệ!", null);
            }

            if (ngaydi < ngayden)
            {
                return response.ResponseError(StatusCodes.Status400BadRequest, "Ngày đi không được nhỏ hơn ngày đến!", null);
            }

            // Lấy danh sách phòng trống
            List<Room> listRoomOk = GetAvailableRooms(request.RoomTypeList, request.CheckInDate, request.CheckOutDate);
            

            foreach(var item in listRoomOk)
            {
                Console.WriteLine(item.RoomNumber);
            }



            // Kiểm tra xem phòng có đủ số lượng không
            foreach (var roomType in request.RoomTypeList)
            {
                var roomTypeEntity = await dbContext.roomTypes.FirstOrDefaultAsync(t => t.RoomTypeName == roomType.RoomTypeName);
                var Total = listRoomOk.Where(x => x.RoomTypeID == roomTypeEntity.Id).Count();

                Console.WriteLine(Total + " " + roomType.Quantity);
                // Kiểm tra xem tổng số phòng có đủ cho yêu cầu không
                if (Total < roomType.Quantity)
                {
                    return response.ResponseError(StatusCodes.Status400BadRequest, $"{roomType.RoomTypeName} không đủ, chỉ còn {Total}", null);
                }
            }


            // Tạo khách hàng mới
            var customer = new Customer
            {
                /*CardNumber = request.CardNumber,*/
                FullName = request.FullName,
                Phone = request.NumberPhone,
                Email = request.Email,
                CreateBooking=DateTime.Now,
                
            };
            dbContext.customers.Add(customer);
            await dbContext.SaveChangesAsync();

            // Tạo booking
            var booking = new Booking
            {
                CustomerID = customer.Id,
                CheckInDate = ngayden.Date.AddHours(14),
                
                CheckOutDate = ngaydi.Date.AddHours(12),
                NumberOfChildren = request.ChildQuantity
            };
            dbContext.bookings.Add(booking);
            await dbContext.SaveChangesAsync();


            foreach(var item in request.RoomTypeList)
            {
                if (item.Quantity == 0)
                {
                    continue;
                }
                var roomTypeEntity = await dbContext.roomTypes.FirstOrDefaultAsync(t => t.RoomTypeName == item.RoomTypeName);
                List<Room> listRoomTempt = listRoomOk.Where(x=>x.RoomTypeID== roomTypeEntity.Id).ToList();
                var roomBookingBill = new RoomBookingBill
                {
                    BookingId=booking.Id,
                    RoomTypeId=roomTypeEntity.Id,
                    Quantity=item.Quantity
                };
                dbContext.roomBookingBills.Add(roomBookingBill);
                await dbContext.SaveChangesAsync();

                foreach (var room in listRoomTempt) {
                    var roomNameBill = new RoomNameBill
                    {
                        RoomBookingBillId = roomBookingBill.Id,
                        NameRoom = room.RoomNumber
                    };
                    dbContext.roomNameBills.Add(roomNameBill);
                    await dbContext.SaveChangesAsync();
                }
            }


            // Tính toán giá phòng và tạo hóa đơn
            var numberOfNights = (ngaydi - ngayden).Days ;
            var totalPriceRoom = request.RoomTypeList.Sum(item => dbContext.roomTypes.FirstOrDefault(x => x.RoomTypeName == item.RoomTypeName).PricePerNight * item.Quantity * numberOfNights);

            var bill = new Bill
            {
                BookingId = booking.Id,
                TotalPrice = totalPriceRoom + (request.ChildQuantity * 100000),
                CreateTime = DateTime.Now
            };
            dbContext.bills.Add(bill);
            await dbContext.SaveChangesAsync();

            return response.ResponseSuccess("Đặt phòng thành công!", converter.EntityToDTO(booking));
        }



        public async Task<string> DeleteBooking(int request)
        {
            var booking = await dbContext.bookings.FirstOrDefaultAsync(x => x.Id == request);
            var bill = await dbContext.bills.FirstOrDefaultAsync(x => x.BookingId == request);
            var ListBillService = await dbContext.billServices.Where(x=>x.BillId==bill.Id).ToListAsync();

            foreach(var item in ListBillService)
            {
                dbContext.billServices.Remove(item);
                await dbContext.SaveChangesAsync();
            }


            dbContext.bookings.Remove(booking);
            dbContext.bills.Remove(bill);
            await dbContext.SaveChangesAsync();
            return "Xóa thành công !";
        }

        public async Task<IQueryable<DTO_Booking>> GetListBooking(int? pageSize, int? pageNumber)
        {
            // Sử dụng giá trị mặc định nếu không có giá trị nào được truyền vào
            int defaultPageSize = 10;
            int defaultPageNumber = 1;

            // Kiểm tra và gán giá trị mặc định nếu không có giá trị được truyền vào
            int size = pageSize ?? defaultPageSize;  // Nếu pageSize là null, sử dụng defaultPageSize
            int number = pageNumber ?? defaultPageNumber; // Nếu pageNumber là null, sử dụng defaultPageNumber

            // Đảm bảo pageSize và pageNumber có giá trị hợp lệ (lớn hơn 0)
            if (size <= 0 || number <= 0)
            {
                throw new ArgumentException("Không được nhỏ hơn 0 !");
            }

            // Truy vấn dữ liệu với phân trang
            var query = dbContext.bookings.OrderByDescending(x => x.Id)
            .Select(x => converter.EntityToDTO(x)) // Chỉ lấy dữ liệu cần thiết
                .Skip((number - 1) * size)
                .Take(size);

            var result = await Task.FromResult(query);

            return query;
        }

        public async Task<ResponseBase> UpdateBooking(Request_UpdateBooking request)
        {
           
           
            var booking = await dbContext.bookings.FirstOrDefaultAsync(x => x.Id == request.ID);
            var customer = await dbContext.customers.FirstOrDefaultAsync(x => x.Id == booking.CustomerID);
           
       
    
            customer.FullName = request.CustomerName;
            customer.Phone = request.NumberPhone;
            customer.CardNumber=request.CardNumber;
          
            dbContext.Update(customer);
            await dbContext.SaveChangesAsync();

            return responseBase.ResponseSuccessBase("Cập nhật đặt phòng thành công !");
        }

        public async Task<ResponseBase> AdminCreateBooking(Request_AdminBooking request)
        {
            bool check1 = true;
            foreach (var item in request.RoomTypeList)
            {
                if (item.Quantity != 0)
                {
                    check1 = false;
                    break;
                }
            }
            if (check1)
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Vui lòng chọn phòng !");
            }

            // Kiểm tra và xử lý các trường hợp dữ liệu không hợp lệ
            DateTime ngayden = request.CheckInDate, ngaydi = request.CheckOutDate;


            if (ngayden < DateTime.UtcNow.AddDays(-1))
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Ngày đến không hợp lệ!");
            }

            if (string.IsNullOrWhiteSpace(request.FullName) || request.NumberPhone == null)
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Tên hoặc số điện thoại không hợp lệ!");
            }

            if (!CheckInput.IsValiEmail(request.Email) || request.NumberPhone != CheckInput.IsValidPhoneNumber(request.NumberPhone))
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Email hoặc số điện thoại không hợp lệ!");
            }

            if (ngaydi < ngayden)
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Ngày đi không được nhỏ hơn ngày đến!");
            }

            // Lấy danh sách phòng trống
            List<Room> listRoomOk = GetAvailableRooms(request.RoomTypeList, request.CheckInDate, request.CheckOutDate);


           


            // Kiểm tra xem phòng có đủ số lượng không
            foreach (var roomType in request.RoomTypeList)
            {
                var roomTypeEntity = await dbContext.roomTypes.FirstOrDefaultAsync(t => t.RoomTypeName == roomType.RoomTypeName);
                var Total = listRoomOk.Where(x => x.RoomTypeID == roomTypeEntity.Id).Count();

                Console.WriteLine(Total + " " + roomType.Quantity);
                // Kiểm tra xem tổng số phòng có đủ cho yêu cầu không
                if (Total < roomType.Quantity)
                {
                    return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, $"{roomType.RoomTypeName} không đủ, chỉ còn {Total}");
                }
            }


            // Tạo khách hàng mới
            var customer = new Customer
            {
                CardNumber = request.CardNumber,
                FullName = request.FullName,
                Phone = request.NumberPhone,
                Email = request.Email,
                CreateBooking = DateTime.Now,

            };
            dbContext.customers.Add(customer);
            await dbContext.SaveChangesAsync();

            // Tạo booking
            var booking = new Booking
            {
                CustomerID = customer.Id,
                CheckInDate = ngayden.Date.AddHours(14),

                CheckOutDate = ngaydi.Date.AddHours(12),
                NumberOfChildren = request.ChildQuantity
            };
            dbContext.bookings.Add(booking);
            await dbContext.SaveChangesAsync();


            foreach (var item in request.RoomTypeList)
            {
                if (item.Quantity == 0)
                {
                    continue;
                }
                var roomTypeEntity = await dbContext.roomTypes.FirstOrDefaultAsync(t => t.RoomTypeName == item.RoomTypeName);
                List<Room> listRoomTempt = listRoomOk.Where(x => x.RoomTypeID == roomTypeEntity.Id).ToList();
                var roomBookingBill = new RoomBookingBill
                {
                    BookingId = booking.Id,
                    RoomTypeId = roomTypeEntity.Id,
                    Quantity = item.Quantity
                };
                dbContext.roomBookingBills.Add(roomBookingBill);
                await dbContext.SaveChangesAsync();

                foreach (var room in listRoomTempt)
                {
                    var roomNameBill = new RoomNameBill
                    {
                        RoomBookingBillId = roomBookingBill.Id,
                        NameRoom = room.RoomNumber
                    };
                    dbContext.roomNameBills.Add(roomNameBill);
                    await dbContext.SaveChangesAsync();
                }
            }


            // Tính toán giá phòng và tạo hóa đơn
            var numberOfNights = (ngaydi - ngayden).Days;
            var totalPriceRoom = request.RoomTypeList.Sum(item => dbContext.roomTypes.FirstOrDefault(x => x.RoomTypeName == item.RoomTypeName).PricePerNight * item.Quantity * numberOfNights);

            var bill = new Bill
            {
                BookingId = booking.Id,
                TotalPrice = totalPriceRoom + (request.ChildQuantity * 100000),
                CreateTime = DateTime.Now,
                
                
            };
            foreach (var item in request.RoomTypeList)
            {
                if (item.Quantity == 0)
                {
                    continue;
                }
                var roomTypeEntity = await dbContext.roomTypes.FirstOrDefaultAsync(t => t.RoomTypeName == item.RoomTypeName);
                List<Room> listRoomTempt = listRoomOk.Where(x => x.RoomTypeID == roomTypeEntity.Id).ToList();
               foreach(var roomTempt in listRoomTempt)
                if (bill.ContentPayAll == null)
                    bill.ContentPayAll = roomTempt.RoomNumber;
                else
                    bill.ContentPayAll += ", " + roomTempt.RoomNumber;
            }
            bill.PayOk = 0;
            bill.ContentPay = "0";
            dbContext.bills.Add(bill);
            await dbContext.SaveChangesAsync();

            return responseBase.ResponseSuccessBase("Đặt phòng thành công!");
        }
    }
}
