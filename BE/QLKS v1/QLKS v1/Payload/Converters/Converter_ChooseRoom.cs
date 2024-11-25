using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_ChooseRoom
    {
        private readonly AppDbContext dbContext;
        
        public Converter_ChooseRoom(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DTO_ChooseRoom EntityToDTO(Room room)
            => new DTO_ChooseRoom
            {
                RoomNumber = room.RoomNumber,
                RoomTypeName = dbContext.roomTypes.Find(room.RoomTypeID).RoomTypeName
            };
    }
}
