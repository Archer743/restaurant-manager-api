using System.Net;

namespace Restaurant_API.Exceptions
{
    public class BadRequestException : HttpException
    {
        public BadRequestException(string errorCode) : base(HttpStatusCode.BadRequest, errorCode) { }
    }
}