using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_Staff
    {
        public DTO_Staff EntityToDTO(User staff)
        {
            return new DTO_Staff
            {
                Id = staff.Id,
                Name=staff.FullName,
                Email=staff.Email,
                Address= staff.Address,
                Phone=staff.NumberPhone,
                Gender=staff.Gender
            };
        }
    }
}
