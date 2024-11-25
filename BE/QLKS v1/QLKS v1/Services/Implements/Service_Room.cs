using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Room;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;
using System;

namespace QLKS_v1.Services.Implements
{
    public class Service_Room : IService_Room
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_ChooseRoom converter_Choose;
        private readonly Converter_Room converter_Room;
        private readonly ResponseBase response;

        public Service_Room(AppDbContext dbContext, Converter_ChooseRoom converter_Choose, Converter_Room converter_Room, ResponseBase response)
        {
            this.dbContext = dbContext;
            this.converter_Choose = converter_Choose;
            this.converter_Room = converter_Room;
            this.response = response;
        }

        public DTO_ViewChiTietRoom ViewChiTietRoom(string RoomName)
        {
            var roomDetails = dbContext.rooms
        .Where(r => r.RoomNumber == RoomName)
        .Select(r => new DTO_ViewChiTietRoom
        {
            RoomName = r.RoomNumber,
            RoomTypeName = r.RoomType.RoomTypeName,
            Price = r.RoomType.PricePerNight,
            Equipments = r.Equipments.Select(e => e.Name).ToList()
        })
        .FirstOrDefault();


            return roomDetails;
        }

        public IQueryable<DTO_ChooseRoom> getListChooseRoom()
        {
            return dbContext.rooms.Select(x => converter_Choose.EntityToDTO(x));
        }

        public IQueryable<DTO_Room> getListRoom()
        {
            return dbContext.rooms.Select(x => converter_Room.EntityToDTO(x));
        }
        



        

        public IQueryable<DTO_Room> getAllRoom(int pageSize, int pageNumber)
        {
            return dbContext.rooms.OrderByDescending(x=>x.Id).AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => converter_Room.GetAllRoom(x));
        }

        public IQueryable<DTO_Room> GetListRoomforCase(string RoomTypeName, int pageSize, int pageNumber)
        {
            if (RoomTypeName == "all")
            {
                return dbContext.rooms.OrderByDescending(x => x.Id).Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x => converter_Room.EntityToDTO(x)).AsQueryable();
            }
            else if (RoomTypeName== "room_active")
            {
                var ListRoomIDActive = dbContext.roomNameBills.Include(x => x.RoomBookingBill)
                    .ThenInclude(x => x.Booking)
                    .ThenInclude(x => x.Customer)
             
                    .AsNoTracking()
                    .Where(x => x.RoomBookingBill.Booking.CheckInDate.Date <= DateTime.UtcNow.Date
                    && x.RoomBookingBill.Booking.CheckOutDate.Date >= DateTime.UtcNow.Date
                    && x.RoomBookingBill.Booking.Customer.CardNumber != null).Select(x=>x.NameRoom).Distinct().ToList();

                List<Room> rooms = new List<Room>();

                foreach(var item in ListRoomIDActive)
                {
                    foreach(var room in dbContext.rooms) {
                        if (room.RoomNumber == item)
                        {
                            rooms.Add(room);
                        }
                        
                    }
                    
                }


                return rooms
     .OrderByDescending(x => x.Id)
     .Skip((pageNumber - 1) * pageSize)
     .Take(pageSize)
     .Select(x => converter_Room.EntityToDTO(x))
     .AsQueryable();
            }
            else
            {
                return dbContext.rooms.OrderByDescending(x => x.Id).Where(x => x.RoomTypeID == dbContext.roomTypes.FirstOrDefault(x => x.RoomTypeName == RoomTypeName).Id).AsQueryable().Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x=>converter_Room.EntityToDTO(x));
            }
        }

        public async Task<IQueryable<DTO_Room>> getListRoomNull(string RoomTypeName, int pageSize, int pageNumber)
        {
            // Lọc loại phòng theo RoomTypeName
            string roomTypeFilter = RoomTypeName switch
            {
                "alone" => "Phòng đơn",
                "double" => "Phòng đôi",
                "vip" => "Phòng vip",
                _ => null
            };

            // Truy vấn để lấy danh sách phòng theo điều kiện loại phòng
            var queryAll = from room in dbContext.rooms
                           join roomType in dbContext.roomTypes on room.RoomTypeID equals roomType.Id
                           // Lọc theo loại phòng nếu có
                           where roomTypeFilter == null || roomType.RoomTypeName == roomTypeFilter
                           select new DTO_Room
                           {
                               Id = room.Id,
                               RoomName = room.RoomNumber,
                               RoomType = roomType.RoomTypeName,
                               Price = roomType.PricePerNight
                           };

            // Áp dụng phân trang
            var pagedResult = queryAll.OrderByDescending(x=>x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsQueryable();

            return await Task.FromResult(pagedResult);
        }
        public async Task<IQueryable<DTO_Room>> getListRoomNullFull(string RoomTypeName, int pageSize, int pageNumber, DateTime checkIn, DateTime checkOut)
        {
            if (RoomTypeName == "all")
            {
                var queryAll = from room in dbContext.rooms
                               join roomType in dbContext.roomTypes on room.RoomTypeID equals roomType.Id
                               // Loại trừ những phòng đã được đặt
                               where !(from rb in dbContext.roomBookingBills
                                       join rnb in dbContext.roomNameBills on rb.Id equals rnb.RoomBookingBillId
                                       join b in dbContext.bookings on rb.BookingId equals b.Id
                                       where (b.CheckInDate < checkOut && b.CheckOutDate > checkIn) // Kiểm tra thời gian phòng đã được đặt
                                       select rnb.NameRoom).Contains(room.RoomNumber) // Sử dụng NameRoom từ bảng roomNameBills để loại trừ
                               select new DTO_Room
                               {
                                   Id = room.Id,
                                   RoomName = room.RoomNumber,
                                   RoomType = roomType.RoomTypeName,
                                   Price = roomType.PricePerNight
                               };

                // Áp dụng phân trang
                return await Task.FromResult(queryAll.OrderByDescending(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsQueryable());
            }
            var query = from room in dbContext.rooms
                        join roomType in dbContext.roomTypes on room.RoomTypeID equals roomType.Id
                        where roomType.RoomTypeName == RoomTypeName
                        // Loại trừ những phòng đã được đặt
                        && !(from rb in dbContext.roomBookingBills
                             join rnb in dbContext.roomNameBills on rb.Id equals rnb.RoomBookingBillId
                             join b in dbContext.bookings on rb.BookingId equals b.Id
                             where (b.CheckInDate < checkOut && b.CheckOutDate > checkIn) // Kiểm tra thời gian phòng đã được đặt
                             select rnb.NameRoom).Contains(room.RoomNumber) // Sử dụng NameRoom từ bảng roomNameBills để loại trừ
                        select new DTO_Room
                        {
                            Id = room.Id,
                            RoomName = room.RoomNumber,
                            RoomType = roomType.RoomTypeName,
                            Price = roomType.PricePerNight
                        };

            // Áp dụng phân trang
            return await Task.FromResult(query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsQueryable());
        }

        public async Task<ResponseBase> CreateRoom(Request_CreateRoom request)
        {
            var roomcheck = await dbContext.rooms.FirstOrDefaultAsync(x => x.RoomNumber == request.RoomName);
            if (roomcheck != null)
                return response.ResponseErrorBase(StatusCodes.Status400BadRequest, "Tên phòng đã tồn tại !");


            var roomtypename= await dbContext.roomTypes.FirstOrDefaultAsync(x=>x.RoomTypeName == request.RoomTypeName);
            var room = new Room()
            {
                RoomNumber = request.RoomName,
                RoomTypeID = roomtypename.Id,
            };
            dbContext.rooms.Add(room);
            await dbContext.SaveChangesAsync();

            return response.ResponseSuccessBase("Thêm phòng mới thành công !");

        }

        public async Task<ResponseBase> UpdateRoom(Request_UpdateRoom request)
        {
           

            var roomtypename = await dbContext.roomTypes.FirstOrDefaultAsync(x => x.RoomTypeName == request.RoomTypeName);
            var room = await dbContext.rooms.FirstOrDefaultAsync(x => x.Id == request.Id);
            var roomcheck = await dbContext.rooms.FirstOrDefaultAsync(x => x.RoomNumber == request.RoomName);
            if (roomcheck != null && roomcheck.RoomNumber!= room.RoomNumber)
                return response.ResponseErrorBase(StatusCodes.Status400BadRequest, "Tên phòng đã tồn tại !");
            room.RoomNumber= request.RoomName;
            room.RoomTypeID = roomtypename.Id;


            dbContext.rooms.Update(room);
            await dbContext.SaveChangesAsync();

            return response.ResponseSuccessBase("Cập nhật phòng thành công !");
        }

        public async Task<ResponseBase> DeleteRoom(int RoomId)
        {
            var room = await dbContext.rooms.FirstOrDefaultAsync(x => x.Id == RoomId);
            dbContext.rooms.Remove(room);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Xóa phòng thành công !");

        }

        public async Task<int> TypeALone()
        {
            int rs = await dbContext.roomBookingBills.Include(x => x.Booking).ThenInclude(x => x.Customer)
                .Where(x => x.Booking.CheckInDate.Date <= DateTime.UtcNow.Date && x.Booking.CheckOutDate.Date >= DateTime.UtcNow.Date && x.Booking.Customer.CardNumber != null && x.RoomTypeId==1)
               
                .SumAsync(x => x.Quantity);
            return rs;
        }

        public async Task<int> TypeDouble()
        {
            int rs = await dbContext.roomBookingBills.Include(x => x.Booking).ThenInclude(x => x.Customer)
                .Where(x => x.Booking.CheckInDate.Date <= DateTime.UtcNow.Date && x.Booking.CheckOutDate.Date >= DateTime.UtcNow.Date && x.Booking.Customer.CardNumber != null && x.RoomTypeId == 2)

                .SumAsync(x => x.Quantity);
            return rs;
        }

        public async Task<int> TypeVip()
        {
            int rs = await dbContext.roomBookingBills.Include(x => x.Booking).ThenInclude(x => x.Customer)
                .Where(x => x.Booking.CheckInDate.Date <= DateTime.UtcNow.Date && x.Booking.CheckOutDate.Date >= DateTime.UtcNow.Date && x.Booking.Customer.CardNumber != null && x.RoomTypeId == 3)

                .SumAsync(x => x.Quantity);
            return rs;
        }
    }
}
