namespace QLKS_v1.Payload.Requests.Request_User
{
    public class Request_ChangePassword
    {
        public string OTP { get; set; }
        public string PassWordNew { get; set; }
        public string PassWordComfirm { get; set; }
    }
}
