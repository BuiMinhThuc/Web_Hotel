using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_User;
using QLKS_v1.Services.Interfaces;

namespace QLKS_v1.Controllers
{
    [Route("")]
    [ApiController]
    public class Controller_Authen : ControllerBase
    {
        private readonly IService_User service_User;
        private readonly IConfiguration _configuration;

        public Controller_Authen(IService_User service_User, IConfiguration configuration)
        {
            this.service_User = service_User;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("Renew-token")]
        public IActionResult RenewToken(DTO_Token token)
        {
            var result = service_User.RenewAccessToken(token);
            if (result == null)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(Request_Login request)
        {
            return Ok(await service_User.Login(request));
        }
        [HttpPost("Get-Otp")]
        public async Task<IActionResult> GetOtpAsync(Request_ForgotAccount request)
        {
            return Ok(await service_User.GetOtpAsync(request));
        } 
        [HttpPut("Change_Pass")]
        public async Task<IActionResult> ChangePasswordAsync(Request_ChangePassword request)
        {
            return Ok(await service_User.ChangePassWordAsync(request));
        }
        [HttpPost("Register")]
        public async Task<IActionResult> ReGisTer(Request_Register request)
        {
            return Ok(await service_User.Resigeter(request));
        }
        [HttpPut("ComfirmEmail")]
        public async Task<IActionResult> ComfirmEmail(Request_ComfirmEmail request)
        {
            return Ok(await service_User.ComfirmOtpAsync(request));
        }
        [HttpGet("ResendOtp{userId}")]
        
        public async Task<IActionResult> ResendOtp(int userId)
        {
            return Ok(await service_User.ResendOtp(userId));
        }
        [HttpGet("GetUrlAvt{userId}")]

        public async Task<IActionResult> getUrlAvt(int userId)
        {
            return Ok(await service_User.getURLAVT(userId));
        }
        [HttpPost("Update{userId}")]
        public async Task<IActionResult> UpdateUserToCustomer(int userId,[FromForm] Request_CustomerUpdateInfor request)
        {
            return Ok(await service_User.UpdateUserToCustomer(userId,request));
        }
        [HttpGet("GetUserbyId{userId}")]

        public async Task<IActionResult> getuserbyId(int userId)
        {
            return Ok(await service_User.GetUsserbyId(userId));
        }
        [HttpGet("GetListUser")]

        public async Task<IActionResult> GetListUser(int pageSize=7, int pageNumber =1)
        {
            return Ok(await service_User.GetListUser(pageSize, pageNumber));
        }
        [HttpPut("ChangePassByUser")]
        public async Task<IActionResult> ChangePassByUser(Request_ChangePassWordByUser request)
        {
            return Ok(await service_User.ChangePassWordByUser(request));
        }
    }
}
