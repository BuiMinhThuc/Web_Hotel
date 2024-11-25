namespace QLKS_v1.Payload.Requests.Request_User
{
    public class Request_ChangePassWordByUser
    {
        public int UserId {  get; set; }
        public string PassOld {  get; set; }
        public string PassNew { get; set; }
        public string PassComfirm { get; set; }
    }
}
