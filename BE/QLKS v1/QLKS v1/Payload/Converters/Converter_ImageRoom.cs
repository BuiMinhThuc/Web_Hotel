using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_ImageRoom
    {
        public DTO_ImageRoom EntityToDTO(RoomImage roomImage)
        {
            return new DTO_ImageRoom
            {   
                Id = roomImage.Id,
                RoomId = roomImage.RoomId,
                UrlImage=roomImage.UrlImage,
            };
        }

        
    }
}
