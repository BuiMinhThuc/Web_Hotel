using Microsoft.EntityFrameworkCore;
using QLKS_v1.DataContext;
using QLKS_v1.Entities;
using QLKS_v1.Handle;
using QLKS_v1.Payload.Converters;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_ImageRoom;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Services.Implements
{
    public class Service_RoomImage : IService_RoomImage
    {
        private readonly AppDbContext dbContext;
        private readonly Converter_ImageRoom converter_ImageRoom;
        private readonly ResponseBase responseBase;

        public Service_RoomImage(AppDbContext dbContext, Converter_ImageRoom converter_ImageRoom, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.converter_ImageRoom = converter_ImageRoom;
            this.responseBase = responseBase;
        }

        public async Task<ResponseBase> CreateImgeRoom(Request_CreateImgRoom request)
        {
            int imageSize = 2 * 1024 * 768;
            if (request.UrlImg != null)
            {
                if (!HandleImage.IsImage(request.UrlImg, imageSize))
                {
                    return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ !");
                }
            }
            string ImgUrl = null;
            var cloudinary = new CloudinaryService();

            if (request.UrlImg == null)
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Vui lòng chọn ảnh !");
            }
            else
            {
                ImgUrl = await cloudinary.UploadImage(request.UrlImg);
            }
            var newimgroom = new RoomImage();
            newimgroom.RoomId = request.RoomId;
            newimgroom.UrlImage = ImgUrl;
            dbContext.roomImages.Add(newimgroom);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Thêm thành công !");

        }

        public async Task<ResponseBase> DeletaImgeRoom(int request)
        {
            var newimgroom = await dbContext.roomImages.FirstOrDefaultAsync(x => x.Id == request);
            var clou = new CloudinaryService();
            
            dbContext.roomImages.Remove(newimgroom);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Xóa thành công !");
        }

        public IQueryable<DTO_ImageRoom> getListAvtRoom()
        {
            return dbContext.roomImages.Select(x => converter_ImageRoom.EntityToDTO(x));
        }

        public async Task<List<DTO_ImageRoom>> ListImgOfRoom(int Id)
        {
            var list = await dbContext.roomImages.Where(x => x.RoomId == Id).Select(x=>new DTO_ImageRoom
            {
                Id=x.Id,
                RoomId=x.RoomId,
                UrlImage=x.UrlImage

            }).ToListAsync();
            return list;
        }

        public async Task<ResponseBase> UpdateImgeRoom(Request_UpdateImgRoom request)
        {
            int imageSize = 2 * 1024 * 768;
            if (request.UrlImg != null)
            {
                if (!HandleImage.IsImage(request.UrlImg, imageSize))
                {
                    return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ !");
                }
            }
            string ImgUrl = null;
            var cloudinary = new CloudinaryService();
            var newimgroom = await dbContext.roomImages.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (request.UrlImg == null)
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Vui lòng chọn ảnh !");
            }
            else
            {
                ImgUrl = await cloudinary.UploadImage(request.UrlImg);
            }
       
            newimgroom.RoomId = request.RoomId;
            newimgroom.UrlImage = ImgUrl;
            dbContext.roomImages.Update(newimgroom);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Sửa thành công !");
        }
    }
}
