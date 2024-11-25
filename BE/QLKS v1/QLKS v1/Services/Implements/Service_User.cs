using Microsoft.EntityFrameworkCore;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Responses;
using QLKS_v1.Services.Interfaces;
using QLKS_v1.Handle;
using QLKS_v1.DataContext;
using BCryptNet = BCrypt.Net.BCrypt;
using QLKS_v1.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Reflection;
using QLKS_v1.Payload.Requests.Request_User;
using Microsoft.AspNetCore.Http;
using Azure.Core;
using SixLabors.ImageSharp;
using System.Text;
using QLKS_v1.Payload.Converters;
using Org.BouncyCastle.Asn1.Ocsp;

namespace QLKS_v1.Services.Implements
{
    public class Service_User:IService_User
    {
        private readonly AppDbContext dbContext;
        private readonly ResponseObject<DTO_Token> responseObject;
        private readonly ResponseObject<DTO_GetUser> responseObjectGetUser;
        private readonly IConfiguration configuration;
        private readonly ResponseBase responseBase;
        private readonly Converter_User converter_User;

        public Service_User(AppDbContext dbContext, ResponseObject<DTO_Token> responseObject, ResponseObject<DTO_GetUser> responseObjectGetUser, IConfiguration configuration, ResponseBase responseBase, Converter_User converter_User)
        {
            this.dbContext = dbContext;
            this.responseObject = responseObject;
            this.responseObjectGetUser = responseObjectGetUser;
            this.configuration = configuration;
            this.responseBase = responseBase;
            this.converter_User = converter_User;
        }

        public ResponseObject<DTO_Token> RenewAccessToken(DTO_Token request)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:SecretKey").Value);

            var tokenValidation = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:SecretKey").Value))
            };

            try
            {
                var tokenAuthentication = jwtTokenHandler.ValidateToken(request.AccessToken, tokenValidation, out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwtSecurityToken || jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Token không hợp lệ", null);
                }
                RefreshToken refreshToken = dbContext.refreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);
                if (refreshToken == null)
                {
                    return responseObject.ResponseError(StatusCodes.Status404NotFound, "RefreshToken không tồn tại trong database", null);
                }
                if (refreshToken.ExpiredTime < DateTime.Now)
                {
                    return responseObject.ResponseError(StatusCodes.Status401Unauthorized, "Token chưa hết hạn", null);
                }
                var user = dbContext.users.FirstOrDefault(x => x.Id == refreshToken.UserId);
                if (user == null)
                {
                    return responseObject.ResponseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
                }
                var newToken = GenerateAccessToken(user);

                return responseObject.ResponseSuccess("Làm mới token thành công", newToken);
            }
            catch (Exception ex)
            {
                return responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
        #region GenerateRefreshToken
        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        #endregion
        #region GenerateAccessToken
        public DTO_Token GenerateAccessToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:SecretKey").Value);

            var decentralization = dbContext.roles.FirstOrDefault(x => x.Id == user.RoleId);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Username", user.UserName.ToString()),
                    new Claim("RoleId", user.RoleId.ToString()),
                    new Claim("NumberPhone", user.NumberPhone.ToString()),
                    //new Claim("UrlAvatar", user.UrlAvatar.ToString()),
                    new Claim("FullName", user.FullName.ToString()),
                    new Claim(ClaimTypes.Role, decentralization?.Code ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            RefreshToken rf = new RefreshToken
            {
                Token = refreshToken,
                ExpiredTime = DateTime.Now.AddHours(4),
                UserId = user.Id
            };

            dbContext.refreshTokens.Add(rf);
            dbContext.SaveChanges();

            DTO_Token tokenDTO = new DTO_Token
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return tokenDTO;
        }
        #endregion

        public async Task<ResponseObject<DTO_Token>> Login(Request_Login request)
        {
            if (string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Password)
             )
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Giá trị nhập không hợp lệ !", null);
            }
            var user =await  dbContext.users.FirstOrDefaultAsync(x => x.UserName == request.Username);
            if (user == null)
                return responseObject.ResponseError( StatusCodes.Status400BadRequest, "UserName hoặc Password không chính xác", null);
            if(user.IsActice==false)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest,"Tài khoản đã bị vô hiệu hóa !",null);
            }
           // var comfirmEM= dbContext.comfirmEmails.FirstOrDefaultAsync(x=>x.UserId==user.Id);
            if (user.UserStatusId == 2)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Tài khoản chưa được kích hoạt, vui lòng kiểm tra email và xác nhận ! !", GenerateAccessToken(user));
            }
            if (!BCryptNet.Verify(request.Password.Trim(), user.Password.Trim()))
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "UserName hoặc Password không chính xác", null);
            }
            if (BCryptNet.Verify(request.Password.Trim(), user.Password.Trim()))
            {
                return responseObject.ResponseSuccess("Đăng nhập thành công", GenerateAccessToken(user));
            }
            return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mất kết nối !", null);

        }

        public async Task<string>  GetOtpAsync(Request_ForgotAccount request)
        {
            if (!CheckInput.IsValiEmail(request.Email))
                return "Email không hợp lệ !";
            var User =await dbContext.users.FirstOrDefaultAsync(x=>x.Email== request.Email);
            if (User == null)
                return "Email này chưa kích hoạt tài khoản !";
            else
            {
                EmailTo emailTo = new EmailTo();
                emailTo.Mail = request.Email;
                emailTo.Subject = "XÁC NHẬN Tài KHOẢN HOTEL";
                Random random = new Random();
                int otp = random.Next(100000, 1000000);
                emailTo.Content = "Mã otp của bạn là:Hotel@" + otp + "\n Mã xác nhận của bạn sẽ hết hạn sau 5 phút nữa !";
                await emailTo.SendEmailAsync(emailTo);
                ComfirmEmail comfirmEmail = new ComfirmEmail();
                comfirmEmail.ConfirmCode = "Hotel@" + otp;
                comfirmEmail.UserId = User.Id;
                dbContext.comfirmEmails.Add(comfirmEmail);
                await dbContext.SaveChangesAsync();
               
                return "Vui lòng kiểm tra Email để lấy mã xác nhận !";
            }    
        }

        public async Task<string> ChangePassWordAsync(Request_ChangePassword request)
        {
            var checkotp =await dbContext.comfirmEmails.FirstOrDefaultAsync(x => x.ConfirmCode == request.OTP);
            if (checkotp is null)
                return "Mã xác nhận không đúng !";
            if (checkotp.ExpiredDateTime < DateTime.Now)
                return "Mã xác nhận đã hết hạn !";
            if (request.PassWordNew != request.PassWordComfirm)
                return "PassWord xác nhận không trung khớp !";
            if (request.PassWordNew != CheckInput.IsPassWord(request.PassWordNew))
                return CheckInput.IsPassWord(request.PassWordNew);
            var user =await dbContext.users.FirstOrDefaultAsync(x => x.Id == checkotp.UserId);
            user.Password = BCryptNet.HashPassword(request.PassWordNew);
            dbContext.users.Update(user);
            await dbContext.SaveChangesAsync();
            return "Đổi Password thành công !";
        }

        public async Task<string> Resigeter(Request_Register request)
        {
            var searchuser = await dbContext.users.FirstOrDefaultAsync(x=>x.UserName== request.UserName);
            if (searchuser != null)
                return "Username đã tồn tại !";
            if (request.UserName != CheckInput.IsValidUsername(request.UserName))
                return CheckInput.IsValidUsername(request.UserName);
            if (request.Password != CheckInput.IsPassWord(request.Password))
                return CheckInput.IsPassWord(request.Password);
            if (!CheckInput.IsValiEmail(request.Email))
                return "Email không hợp lệ !";
            if (request.NumberPhone != CheckInput.IsValidPhoneNumber(request.NumberPhone))
                return CheckInput.IsValidPhoneNumber(request.NumberPhone);
            int imageSize = 2 * 1024 * 768;
            if (request.UrlAvt != null)
            {
                if ( !HandleImage.IsImage(request.UrlAvt, imageSize))
                {
                    return  "Ảnh không hợp lệ !";
                }
            }
            var cloudinary = new CloudinaryService();
            // Tải lên ảnh trước khi tạo đối tượng User
            string avatarUrl = null;
            if(request.UrlAvt == null)
                avatarUrl= "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=";
            if (request.UrlAvt != null)
            {
                avatarUrl = await cloudinary.UploadImage(request.UrlAvt);
            }
            var user = new User()
            {
                UserName = request.UserName,
                Password = BCryptNet.HashPassword(request.Password),
                Email = request.Email,
                FullName = request.FullName,
                NumberPhone = request.NumberPhone,
                UrlAvatar = avatarUrl,
                Address = request.Address,
                Gender= "Nam",
            };
            dbContext.users.Add(user);
            await dbContext.SaveChangesAsync();
            EmailTo emailTo = new EmailTo();
            emailTo.Mail = request.Email;
            emailTo.Subject = "XÁC NHẬN Tài KHOẢN FLORENTINO";
            Random random = new Random();
            int otp = random.Next(100000, 1000000);
            emailTo.Content = "Mã otp của bạn là:Hotel@" + otp + "\n Mã xác nhận của bạn sẽ hết hạn sau 5 phút nữa !";
            await emailTo.SendEmailAsync(emailTo);
            ComfirmEmail comfirmEmail = new ComfirmEmail();
            comfirmEmail.ConfirmCode = "Hotel@" + otp;
            comfirmEmail.UserId = user.Id;
            dbContext.comfirmEmails.Add(comfirmEmail);
            await dbContext.SaveChangesAsync();
            return "Đăng ký tài khoản thành công !";
        }

        public async Task<string> ComfirmOtpAsync(Request_ComfirmEmail request)
        {
            var checkotp = await dbContext.comfirmEmails.FirstOrDefaultAsync(x => x.ConfirmCode == request.code);
            if (checkotp is null)
                return "Mã xác nhận không đúng !";
            if (checkotp.ExpiredDateTime < DateTime.Now)
                return "Mã xác nhận đã hết hạn !";
            var user = await dbContext.users.FirstOrDefaultAsync(x => x.Id == checkotp.UserId);
            user.UserStatusId = 1;
            dbContext.users.Update(user);
            await dbContext.SaveChangesAsync();
            return "Kích hoạt tài khoản thành công !";
        }

        public async Task<string> ResendOtp(int userId)
        {
            var userConfirmation = await dbContext.comfirmEmails.FirstOrDefaultAsync(x => x.UserId == userId);
            var user = await dbContext.users.FirstOrDefaultAsync(x => x.Id == userId);
            EmailTo emailTo = new EmailTo();
            emailTo.Mail = user.Email;
            emailTo.Subject = "XÁC NHẬN Tài KHOẢN FLORENTINO";
            Random random = new Random();
            int otp = random.Next(100000, 1000000);
            emailTo.Content = "Mã otp của bạn là:Hotel@" + otp + "\n Mã xác nhận của bạn sẽ hết hạn sau 5 phút nữa !";
            await emailTo.SendEmailAsync(emailTo);
            ComfirmEmail comfirmEmail = new ComfirmEmail();
            comfirmEmail.ConfirmCode = "Hotel@" + otp;
            comfirmEmail.UserId = user.Id;
            dbContext.comfirmEmails.Update(comfirmEmail);
            await dbContext.SaveChangesAsync();
            return "Gủi thành công !";

        }

        public async Task<string> getURLAVT(int userId)
        {
            var user= await dbContext.users.FirstOrDefaultAsync(x=>x.Id == userId);
            if (user.UrlAvatar != null)
            {
                return user.UrlAvatar;
            }
            else
                return "https://icons.iconarchive.com/icons/papirus-team/papirus-status/256/avatar-default-icon.png";
        }

        public async Task<ResponseBase> UpdateUserToCustomer(int userId, Request_CustomerUpdateInfor request)
        {
            int imageSize = 2 * 1024 * 768;
            if (request.UrlAvatar != null)
            {
                if (!HandleImage.IsImage(request.UrlAvatar, imageSize))
                {
                    return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest,"Ảnh không hợp lệ !");
                }
            }
            var user = await dbContext.users.FirstOrDefaultAsync(x => x.Id == userId);
            user.FullName = request.FullName;
            if (!CheckInput.IsValiEmail(request.Email))
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Email không hợp lệ !");
            if(CheckInput.IsValidPhoneNumber(request.NumberPhone)!=request.NumberPhone)
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Số điện thoại không hợp lệ !");
            user.Email = request.Email;
            user.Address = request.Address;
            user.NumberPhone = request.NumberPhone;
            string avatarUrl = null;
            var cloudinary = new CloudinaryService();
            //if (request.UrlAvatar == null)
            //    avatarUrl = user.UrlAvatar;
            //if (request.UrlAvatar != null)
            //{
            //    avatarUrl = await HandleUploadImage.UpdateFile(user.UrlAvatar,request.UrlAvatar);
            //}


            if(request.UrlAvatar == null) {
                avatarUrl = user.UrlAvatar;
            }
            else
            {
                /*avatarUrl= await cloudinary.ReplaceImage(user.UrlAvatar,request.UrlAvatar);*/
                avatarUrl= await cloudinary.UploadImage(request.UrlAvatar);
            }
            user.UrlAvatar= avatarUrl;
            user.Gender = request.Gender;
            dbContext.users.Update(user);
            await dbContext.SaveChangesAsync();
            return  responseBase.ResponseSuccessBase(GenerateAccessToken(user).AccessToken);


        }

        public async Task<ResponseObject<DTO_GetUser>> GetUsserbyId(int userId)
        {
            var user = await dbContext.users.FirstOrDefaultAsync(x=>x.Id == userId);
            return responseObjectGetUser.ResponseSuccess("Lấy thành công !", converter_User.EntityToDTO(user));
        }

        public async Task<IQueryable<DTO_GetUser>> GetListUser(int pageSize, int pageNumber)
        {
            return await Task.FromResult(dbContext.users.AsNoTracking().Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x=>converter_User.EntityToDTO(x)));
        }

        public  async Task<ResponseBase> UpdateUser(Request_UpdateUser request)
        {

            if (!CheckInput.IsValiEmail(request.Email))
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Email không hợp lệ !");
            if (CheckInput.IsValidPhoneNumber(request.NumberPhone) != request.NumberPhone)
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Số điện thoại không hợp lệ !");
            int imageSize = 2 * 1024 * 768;
            if (request.UrlAvatar != null)
            {
                if (!HandleImage.IsImage(request.UrlAvatar, imageSize))
                {
                    return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ !");
                }
            }



            var user = await dbContext.users.FirstOrDefaultAsync(x => x.Id == request.id);
            var role = await dbContext.roles.FirstOrDefaultAsync(x => x.RoleName == request.RoleName);
            

            user.UserName= request.UserName;
            user.FullName=request.FullName;
            user.Email= request.Email;
            user.Address= request.Address;
            user.NumberPhone= request.NumberPhone;
            user.RoleId = role.Id;
            user.IsActice = request.IsActice;
            string avatarUrl = null;
            var cloudinary = new CloudinaryService();

            if (request.UrlAvatar == null)
            {
                avatarUrl = user.UrlAvatar;
            }
            else
            {
                avatarUrl = await cloudinary.ReplaceImage(user.UrlAvatar,request.UrlAvatar);
               // avatarUrl = await cloudinary.UploadImage(request.UrlAvatar);
            }
            user.UrlAvatar = avatarUrl;
            dbContext.users.Update(user);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase($"Cập nhật thông tin người dùng có ID: {user.Id} thành công !");


        }

        public async Task<ResponseBase> DeleteUser(int UserId)
        {
            var user = await dbContext.users.FirstOrDefaultAsync(x => x.Id == UserId);
            dbContext.users.Remove(user);
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase($"Xóa người dùng có ID: {user.Id} thành công !");
        }

        public async Task<ResponseBase> ChangePassWordByUser(Request_ChangePassWordByUser request)
        {
            var user = await dbContext.users.FirstOrDefaultAsync(x=>x.Id== request.UserId);
            if ( !BCryptNet.Verify( request.PassOld,user.Password))
            {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Mật khẫu hiện tại không đúng !");
            }
            if(CheckInput.IsPassWord(request.PassNew)!=request.PassNew) {
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Mật khẫu phải gồm kí hiệu đặc biệt chữ hoa và số !");

            }
            if(request.PassNew != request.PassComfirm)
                return responseBase.ResponseErrorBase(StatusCodes.Status400BadRequest, "Mật khẫu xác nhận không trùng khớp !");
            user.Password = BCryptNet.HashPassword(request.PassNew);
            dbContext.users.Update(user) ;
            await dbContext.SaveChangesAsync();
            return responseBase.ResponseSuccessBase("Đổi mật khẩu thành công !");
        }
    }
}


