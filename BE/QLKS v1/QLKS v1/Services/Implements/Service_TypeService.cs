using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_TypeService;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_TypeService : IService_TypeService
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_TypeService converter;
        private readonly ResponseBase responseBase;

        public Service_TypeService(AppDbContext dbContext, Converter_TypeService converter, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.converter = converter;
            this.responseBase = responseBase;
        }

        public async Task<IQueryable<DTO_ChoonseTypeService>> AdminChooseTypeServices(int pageSize, int pageNumber)
        {
            return await Task.FromResult(dbContext.serviceTypes.OrderByDescending(x=>x.Id).Skip((pageNumber-1)*pageSize).Take(pageSize).AsNoTracking().Select(x => converter.dTO_ChoonseTypeService(x)));
        }

        public async Task<IQueryable<DTO_ChoonseTypeService>> ChooseTypeServices()
        {
            return await Task.FromResult(dbContext.serviceTypes.AsNoTracking().Select(x => converter.dTO_ChoonseTypeService(x)));
        }

        public async Task<ResponseBase> CreateTypeService(Request_CreateServiceType request)
        {
            var typeService = new ServiceType()
            {
                Name = request.NameType
            };
            dbContext.serviceTypes.Add(typeService);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Thêm loại dịch vụ thành công !");
        }

        public async Task<ResponseBase> DeleteTypeService(int request)
        {

            var typeService = await dbContext.serviceTypes.FirstOrDefaultAsync(x => x.Id == request);
           
            dbContext.serviceTypes.Remove(typeService);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Xóa loại dịch vụ thành công !");
        }

        public async Task<ResponseBase> UpdateTypeService(Request_UpdateTypeService request)
        {
            var typeService = await dbContext.serviceTypes.FirstOrDefaultAsync(x => x.Id == request.Id);
            typeService.Name= request.NameType;
            dbContext.serviceTypes.Update(typeService);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Sửa loại dịch vụ thành công !");
        }
    }
}
