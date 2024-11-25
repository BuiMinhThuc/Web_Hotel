using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_getRoomTypeName
    {
        public DTO_getRoomTypeName EntityToDTO(RoomType roomType)
            => new DTO_getRoomTypeName
            {
                RoomTypename = roomType.RoomTypeName
            };


        public DTO_RoomType AdminGetShow(RoomType roomType)
        {
            return new DTO_RoomType
            {
                Description = roomType.Description,
                RoomTypeName = roomType.RoomTypeName,
                PricePerNight = roomType.PricePerNight,
                Id = roomType.Id,
                QuantityAdult = roomType.QuantityAdult,
                
            };
        }
    }
}
