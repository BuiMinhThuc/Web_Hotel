using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Service;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_Service:IService_Service
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_ChoonseService choonseService;
        private readonly Converter_Service converter_Service;
        private readonly ResponseBase responseBase;

        public Service_Service(AppDbContext dbContext, Converter_ChoonseService choonseService, Converter_Service converter_Service, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.choonseService = choonseService;
            this.converter_Service = converter_Service;
            this.responseBase = responseBase;
        }

        public IQueryable<DTO_ChoonseService> GetChoonseService()
        {
            return dbContext.services.Select(x => choonseService.EntityToDTO(x));
        }
        public IQueryable<DTO_Service> GetAllService(int pageSize, int pageNumber)
        {
            return dbContext.services.OrderByDescending(x=>x.Id).AsNoTracking().Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x => converter_Service.EntityToDTO(x));
        }

        public async Task<ResponseBase> CreateService(Request_CreateService request)
        {


          
            
            var type= await dbContext.serviceTypes.FirstOrDefaultAsync(x=>x.Name == request.TypeService);
            if(type == null) {
                return responseBase.ResponseErrorBase( StatusCodes.Status400BadRequest,"Không tìm thấy loại dịch vụ!");
            }

            var service = new Service();
            service.Price = request.Price;
            service.Name= request.Name;
            service.ServiceTypeId = type.Id;

            dbContext.services.Add(service);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Thêm dịch vụ thành công !");


        }

        public async Task<ResponseBase> UpdateService(Request_UpdateService request)
        {


            var type = await dbContext.serviceTypes.FirstOrDefaultAsync(x => x.Name == request.TypeService);
            if (type == null)
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Không tìm thấy loại dịch vụ!");
            }
            var service = await dbContext.services.FirstOrDefaultAsync(x=>x.Id==request.Id);
            service.Price = request.Price;
            service.Name = request.Name;
            service.ServiceTypeId = type.Id;

            dbContext.services.Update(service);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Cập nhật dịch vụ thành công !");
        }

        public async Task<ResponseBase> DeleteService(int Id)
        {

            var service = await dbContext.services.FirstOrDefaultAsync(x => x.Id == Id);
            dbContext.services.Remove(service);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase($"Xóa dịch vụ ({service.Name}) thành công !");
        }
    }
}

