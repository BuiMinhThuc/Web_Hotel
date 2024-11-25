using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;

namespace QLKS_v1.Payload.Converters
{
    public class Converter_News
    {
        public DTO_News EntityToDTO(New news){
            return new DTO_News
            {
                ID= news.Id,
                Title=news.Title,
                UrlImg=news.UrlImg,
                Content=news.Content
            };
            }
    }
}
