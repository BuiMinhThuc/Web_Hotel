using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_ChoonseService
    {
        public DTO_ChoonseService EntityToDTO(Service service)
        {
            return new DTO_ChoonseService
            {
                Name = service.Name,
                Price = service.Price,
            };
        }
    }
}
