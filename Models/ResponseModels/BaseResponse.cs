using System.Collections.Generic;

namespace Models.ResponseModels
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
        }
        public BaseResponse(T data, string message = null)
        {
            Message = message;
            Data = data;
        }
        public BaseResponse(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
