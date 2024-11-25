using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_Equitment
    {
        public DTO_EquementForId DTO_EquementForId(Equipment equipment)
        {
            return new DTO_EquementForId
            {
                Id = equipment.Id,
                RoomId = equipment.RoomId,
                Name = equipment.Name,
            };
        }
    }
}
