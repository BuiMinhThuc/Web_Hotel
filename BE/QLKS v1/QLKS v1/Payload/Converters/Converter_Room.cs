using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_Room
    {
        private readonly AppDbContext dbContext;
  

        public Converter_Room(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DTO_Room EntityToDTO(Room room)
        {
            var result =dbContext.roomTypes.FirstOrDefault(x => x.Id == room.RoomTypeID);
            return new DTO_Room
            {
                Id = room.Id,
                RoomName=room.RoomNumber,
                RoomType= result.RoomTypeName,
                Price=result.PricePerNight
            };
        }

        public DTO_Room GetAllRoom(Room room)
        {
            var result = dbContext.roomTypes.FirstOrDefault(x => x.Id == room.RoomTypeID);
            return new DTO_Room
            {
                Id = room.Id,
                RoomName = room.RoomNumber,
                RoomType = dbContext.roomTypes.Include(x => x.Rooms).AsNoTracking().Where(x => x.Id == room.RoomTypeID).Select(x => x.RoomTypeName).FirstOrDefault(),
                Price = dbContext.roomTypes.FirstOrDefault(x => x.Id == room.RoomTypeID).PricePerNight
            };
        }



        public DTO_RoomHot RoomHot(Room room)
        {
            var result = dbContext.roomTypes.FirstOrDefault(x => x.Id == room.RoomTypeID);
            return new DTO_RoomHot
            {
                id = room.Id,
                 RoomNumber= room.RoomNumber,
                RoomTypeName = result.RoomTypeName,
                RoomImages=dbContext.roomImages.Where(x=>x.RoomId==room.Id).Select(x=>x.UrlImage).ToList(),
            };
        }
    }
}
