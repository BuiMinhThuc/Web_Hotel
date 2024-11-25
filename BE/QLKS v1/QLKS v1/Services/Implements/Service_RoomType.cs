using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_RoomType;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_RoomType : IService_RoomType
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_getRoomTypeName converter_RTN;
        private readonly ResponseBase response;
        

        public Service_RoomType(AppDbContext dbContext, Converter_getRoomTypeName converter_RTN, ResponseBase response)
        {
            this.dbContext = dbContext;
            this.converter_RTN = converter_RTN;
            this.response = response;
        }

        public IQueryable<DTO_RoomType> AdmingetRoomType(int pageSize, int pageNumber)
        {
            return dbContext.roomTypes.AsNoTracking().Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x=> converter_RTN.AdminGetShow(x)).AsQueryable();
        }

        public async Task<ResponseBase> CreateRooomType(Request_CreateRoomType request)
        {
            var roomType = new RoomType();
            roomType.QuantityAdult = request.QuantityAdult;
            roomType.Description = request.Description;
            roomType.PricePerNight= request.PricePerNight;
            roomType.RoomTypeName = request.RoomTypeName;
            dbContext.roomTypes.Add(roomType);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Thêm loại phòng thành công !");
        }

        public async Task<ResponseBase> DeleteRooomType(int request)
        {
            var roomType = await dbContext.roomTypes.FirstOrDefaultAsync(x => x.Id == request);
          
            dbContext.roomTypes.Remove(roomType);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Xóa loại phòng thành công !");
        }

        public IQueryable<DTO_getRoomTypeName> getListRoomTypenam()
        {
            var result = dbContext.roomTypes.Select(r=>converter_RTN.EntityToDTO(r)).AsQueryable();
            return result;
        }

        public async Task<DTO_TypeRoomHot> getListRoomTypeNameHot() // Tuần/tháng/năm
        {
           

            // Truy vấn với Include và ThenInclude để kiểm tra ngày từ bảng Bills
            var roomTypeWithMaxQuantity = await dbContext.roomBookingBills
                .Include(rbb => rbb.Booking)
                .ThenInclude(b => b.Bills)
             
                .GroupBy(b => b.RoomTypeId)
                .Select(g => new
                {
                    RoomTypeId = g.Key,
                    TotalQuantity = g.Sum(b => b.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .FirstOrDefaultAsync();

            if (roomTypeWithMaxQuantity == null) return null;

            // Lấy thông tin loại phòng từ bảng roomTypes
            var roomType = await dbContext.roomTypes.FirstOrDefaultAsync(x => x.Id == roomTypeWithMaxQuantity.RoomTypeId);

            // Trả về DTO kết quả
            return new DTO_TypeRoomHot
            {
                TypeRoomHot = roomType.RoomTypeName,
                quantity = roomTypeWithMaxQuantity.TotalQuantity
            };
        }

        public async Task<ResponseBase> UpdateRooomType(Request_UpdateRoomType request)
        {
            var roomType = await dbContext.roomTypes.FirstOrDefaultAsync(x => x.Id == request.Id);
            roomType.QuantityAdult = request.QuantityAdult;
            roomType.Description = request.Description;
            roomType.PricePerNight = request.PricePerNight;
            roomType.RoomTypeName = request.RoomTypeName;
            dbContext.roomTypes.Update(roomType);
            await dbContext.SaveChangesAsync();
            return response.ResponseSuccessBase("Sửa loại phòng thành công !");
        }
    }
}
