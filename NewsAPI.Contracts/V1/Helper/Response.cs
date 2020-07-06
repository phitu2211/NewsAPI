using System.Collections.Generic;

namespace NewsAPI.Contracts.V1.Helper
{
    public class Response<T>
    {
        public Response() { }

        public Response(string status, IEnumerable<string> message = null, T data = default, int totalData = 0)
        {
            Status = status;
            Message = message;
            Data = data;
            TotalData = totalData;
        }

        public T Data { get; set; }

        public string Status { get; set; }

        public int TotalData { get; set; }

        public IEnumerable<string> Message { get; set; }
    }
}
