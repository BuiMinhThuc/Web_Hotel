using QLKS_v1.Entities;
using QLKS_v1.Payload.DTOs;
using QLKS_v1.Payload.Requests.Request_User;
using QLKS_v1.Payload.Responses;

namespace QLKS_v1.Services.Interfaces
{
    public interface IService_User
    {
        public Task<string> ComfirmOtpAsync(Request_ComfirmEmail request);
        public Task<string> Resigeter(Request_Register request);
        public ResponseObject<DTO_Token> RenewAccessToken(DTO_Token request);
        public Task<ResponseObject<DTO_Token>> Login(Request_Login request);
        public Task<string> GetOtpAsync(Request_ForgotAccount request);

        public  Task<string> ChangePassWordAsync(Request_ChangePassword request);
        public Task<string> ResendOtp(int userId);
        public Task<string> getURLAVT(int userId);
        public Task<ResponseBase> UpdateUserToCustomer(int userId, Request_CustomerUpdateInfor request);
        public Task<ResponseObject<DTO_GetUser>> GetUsserbyId(int userId);
        Task<IQueryable<DTO_GetUser>> GetListUser(int pageSize, int pageNumber);
        Task<ResponseBase> UpdateUser(Request_UpdateUser request);
        Task<ResponseBase> DeleteUser(int UserId);
        Task<ResponseBase> ChangePassWordByUser(Request_ChangePassWordByUser request);


    }
}
