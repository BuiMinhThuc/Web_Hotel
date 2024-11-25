using SixLabors.ImageSharp.PixelFormats;
namespace QLKS_v1.Handle
{
    public class HandleImage
    {
        public static bool IsImage(IFormFile imageFile, int maxSizeInBytes = (2 * 1024 * 768))
        {
            try
            {
                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageFile.OpenReadStream()))
                {
                    if (image.Width > 0 && image.Height > 0)
                    {
                        if (imageFile.Length <= maxSizeInBytes)
                        {
                            return true;
                        }
                        if (imageFile.Length > maxSizeInBytes)
                        {
                            throw new NotImplementedException("Kích thước file quá lớn");
                        }
                    }
                }
            }
            catch
            {
                if (imageFile.Length > maxSizeInBytes)
                {
                    throw new NotImplementedException("Kích thước file quá lớn");
                }
                else
                {
                    throw new NotImplementedException("File này không phải file có định dạng ảnh");

                }
            }
            return false;
        }
    }
}
