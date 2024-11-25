using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Handle;
using QLKS_v1.Payload.Requests.Request_News;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_News : IService_News
    {
        private readonly AppDbContext dbContext;
        private readonly ResponseBase responseBase;

        public Service_News(AppDbContext dbContext, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.responseBase = responseBase;
        }

        public async Task<ResponseBase> CreateNews(Request_CreateNews request)
        {
          
                

            int imageSize = 2 * 1024 * 768;
            if (request.UrlImg != null)
            {
                if (!HandleImage.IsImage(request.UrlImg, imageSize))
                {
                    return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ !");
                }
            }
            string avatarUrl = null;
            var cloudinary = new CloudinaryService();

            if (request.UrlImg == null)
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Vui lòng chọn ảnh !");
            }
            else
            {
                avatarUrl = await cloudinary.UploadImage(request.UrlImg);
            }
            var news = new New();
            news.Title = request.Title;
            news.Content= request.Content;
            news.UrlImg = avatarUrl;
            dbContext.news.Add(news);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Thêm tin tức thành công !");

        }

        public async Task<ResponseBase> DeleteNews(int request)
        {
            var news = await dbContext.news.FirstOrDefaultAsync(x => x.Id == request);
            dbContext.news.Remove(news);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Xóa tin tức thành công !");
        }

        public  IQueryable<New> GetNews()
        {
            return dbContext.news;
        }


        public IQueryable<New> GetNewsPT(int? pageSize, int? pageNumber)
        {
            return dbContext.news.OrderByDescending(x=>x.Id).Skip((pageNumber.Value-1)*pageSize.Value).Take(pageSize.Value).AsQueryable();
        }

        public async Task<ResponseBase> UpdateNews(Request_UpdateNews request)
        {

            int imageSize = 2 * 1024 * 768;
            if (request.UrlImg != null)
            {
                if (!HandleImage.IsImage(request.UrlImg, imageSize))
                {
                    return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ !");
                }
            }
            var news = await dbContext.news.FirstOrDefaultAsync(x => x.Id == request.Id);
            string avatarUrl = null;
            var cloudinary = new CloudinaryService();

            if (request.UrlImg == null)
            {
                avatarUrl = news.UrlImg;
            }
            else
            {
                avatarUrl = await cloudinary.UploadImage(request.UrlImg);
            }
            
            news.Title = request.Title;
            news.Content = request.Content;
            news.UrlImg = avatarUrl;
            dbContext.news.Update(news);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Cập nhật tin tức thành công !");
        }
    }
}
