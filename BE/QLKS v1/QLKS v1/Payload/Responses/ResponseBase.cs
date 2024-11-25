namespace QLKS_v1.Payload.Responses
{
    public class ResponseBase
    {
        public int Status { get; set; }
        public string Message { get; set; }
 
        public ResponseBase() { }

        public ResponseBase(int status, string message)
        {
            Status = status;
            Message = message;
        }

        public ResponseBase ResponseSuccessBase(string message)
            => new ResponseBase(StatusCodes.Status200OK, message);
        public ResponseBase ResponseErrorBase(int status, string message)
            => new ResponseBase(status, message);
    }
}
