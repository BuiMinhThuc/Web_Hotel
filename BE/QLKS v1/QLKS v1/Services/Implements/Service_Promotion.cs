using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Payload.Requests.Request_Promotion;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_Promotion : IService_Promotion
    {
        private readonly AppDbContext dbContext;
        private readonly ResponseBase responseBase;

        public Service_Promotion(AppDbContext dbContext, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.responseBase = responseBase;
        }

        public async Task<ResponseBase> CreatePromotion(Request_CreatePromotion request)
        {
            var promotion = new Promotion();
            promotion.Start=request.Start; promotion.End=request.End;
            promotion.PriceOff=request.PriceOff;
            dbContext.promotions.Add(promotion);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Thêm khuyến mãi thành công !");
        }

        public async Task<ResponseBase> DeletePromotion(int request)
        {
            var promotion = await dbContext.promotions.FirstOrDefaultAsync(x => x.Id == request);
        
            dbContext.promotions.Remove(promotion);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Xóa khuyến mãi thành công !");
        }

        public IQueryable<Promotion> getListPromotion()
        {
            int pageSize = 3;
            int pageNumber = 1;

            var list = dbContext.promotions.OrderByDescending(x=>x.Id).Skip((pageNumber-1)*pageSize).Take(pageSize).AsQueryable().AsNoTracking();
            return list;

        }
        public async Task<IQueryable<Promotion>> getListPromotionAdmin(int pageSize, int pageNumber )
        {
            
           

            var list =  dbContext.promotions.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).AsQueryable().AsNoTracking();
            return await Task.FromResult( list);

        }

        public async Task<ResponseBase> UpdatePromotion(Request_UpdatePromotion request)
        {
            var promotion = await dbContext.promotions.FirstOrDefaultAsync(x => x.Id == request.Id);
            promotion.Start = request.Start; promotion.End = request.End;
            promotion.PriceOff = request.PriceOff;
            dbContext.promotions.Update(promotion);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Cập nhật khuyến mãi thành công !");
        }
    }
}
