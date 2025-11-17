using System.Net;

namespace Restaurant_API.Exceptions
{
    public class NotFoundException : HttpException
    {
        public NotFoundException(string errorCode) : base(HttpStatusCode.NotFound, errorCode) { }
    }
}
