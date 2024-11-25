using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;
using System.Reflection.Metadata.Ecma335;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_Service
    {
        private readonly AppDbContext dbContext;

        public Converter_Service(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DTO_Service EntityToDTO (Service service)
        {
            return new DTO_Service
            {
                Id = service.Id,
                Name = service.Name,
                TypeName=dbContext.serviceTypes.FirstOrDefault(x=>x.Id==service.ServiceTypeId).Name,
                Price=service.Price,

            };
        }
    }
}
