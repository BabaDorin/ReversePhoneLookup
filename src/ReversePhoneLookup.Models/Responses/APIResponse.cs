namespace ReversePhoneLookup.Models.Responses
{
    public class APIResponse
    {
        public StatusCode StatusCode { get; set; }
        public object Data { get; set; }

        public APIResponse()
        {

        }

        public APIResponse(StatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public APIResponse(StatusCode statusCode, object data)
        {
            StatusCode = statusCode;
            Data = data;
        }
    }
}