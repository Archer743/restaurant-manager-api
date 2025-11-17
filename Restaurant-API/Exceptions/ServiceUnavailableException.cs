using System.Net;

namespace Restaurant_API.Exceptions
{
    public class ServiceUnavailableException : HttpException
    {
        public ServiceUnavailableException(string errorCode) : base(HttpStatusCode.ServiceUnavailable, errorCode) { }
    }
}
