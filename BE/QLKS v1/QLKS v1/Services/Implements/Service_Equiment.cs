using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_Equipment;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_Equiment : IService_Equiment
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_Equitment converter_Equitment;
        private readonly ResponseBase responseBase;

        public Service_Equiment(AppDbContext dbContext, Converter_Equitment converter_Equitment, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.converter_Equitment = converter_Equitment;
            this.responseBase = responseBase;
        }

        public async Task<ResponseBase> CreateEquipment(Request_CreateEquipment request)
        {
            var equipment = new Equipment();
            equipment.RoomId = request.RoomId;
            equipment.Name = request.NameEquipment;
            dbContext.equipments.Add(equipment);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Thêm thành công !");
        }

        public async Task<ResponseBase> DeleteEquipment(int request)
        {
            var equipment = await dbContext.equipments.FirstOrDefaultAsync(x => x.Id == request);
      
            dbContext.equipments.Remove(equipment);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Xóa thành công !");
        }

        public async Task<IQueryable<DTO_EquementForId>> GetListEquipmentForId(int Id)
        {
           return await Task.FromResult(dbContext.equipments.Where(x=>x.RoomId == Id).Select(x=>converter_Equitment.DTO_EquementForId(x)).AsNoTracking());
        }

        public async Task<ResponseBase> UpdateEquipment(Request_UpdateEquipment request)
        {
            var equipment = await dbContext.equipments.FirstOrDefaultAsync(x => x.Id == request.Id);
            equipment.RoomId = request.RoomId;
            equipment.Name = request.NameEquipment;
            dbContext.equipments.Update(equipment);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Sửa thành công !");
        }
    }
}
