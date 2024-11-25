namespace QLKS_v1.Payload.Responses
{
    public class ResponseToken
    {
        public string TokenType { get; set; } = "Bearer";
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
