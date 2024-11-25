using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_TypeService
    {
        public DTO_ChoonseTypeService dTO_ChoonseTypeService(ServiceType serviceType)
        {
            return new DTO_ChoonseTypeService
            {
                id = serviceType.Id,
                TypeServiceName= serviceType.Name
            };
        }
    }
}
