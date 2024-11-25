using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_BookingService
    {
        private readonly AppDbContext dbContext;

        public Converter_BookingService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public DTO_BookingService EntityToDTO(BillService billService)
        {
            return new DTO_BookingService
            {
                Id = billService.Id,
                RoomName= billService.RoomName,
                BillId = billService.BillId,
                Quantity = billService.Quantity,
                ServiceName = dbContext.services.FirstOrDefault(x => x.Id == billService.ServiceId).Name,
                CreateTime = billService.CreateTime,
                Status=billService.Status
                
            };
        }


       
    }
}
