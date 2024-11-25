using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace QLKS_v1.Handle
{
    public class HandleUploadImage
    {
        static string cloudName = "dldk6cqnw";
        static string apiKey = "848167826522825";
        static string apiSecret = "myxNMlwtnnb-6b_S95Gz2kf2ukw";
        static public Account account = new Account(cloudName, apiKey, apiSecret);
        static public Cloudinary _cloudinary = new Cloudinary(account);
        public static async Task<string> Upfile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Không có tập tin được chọn.");
            }
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = "xyz-abc" + "_" + DateTime.Now.Ticks + "image",
                    Transformation = new Transformation().Width(300).Height(400).Crop("fill")
                };
                var uploadResult = await HandleUploadImage._cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }
                string imageUrl = uploadResult.SecureUrl.ToString();
                return imageUrl;
            }
        }
        public static async Task DeleteFile(string url)
        {
            Uri uri = new Uri(url);
            string path = uri.Segments[5];
            int dotIndex = path.LastIndexOf('.');
            if (dotIndex >= 0)
            {
                path = path.Substring(0, dotIndex);
            }
            var deleteParams = new DeletionParams(path)
            {
                ResourceType = ResourceType.Image
            };
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
            if (deleteResult.Error != null)
            {
                throw new Exception(deleteResult.Error.Message);
            }
        }
        public static async Task<string> UpdateFile(string url, IFormFile file)
        {
            await HandleUploadImage.DeleteFile(url);
            string link = await HandleUploadImage.Upfile(file);
            return link;
        }
    }
}
