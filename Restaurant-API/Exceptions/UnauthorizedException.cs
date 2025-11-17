using System.Net;

namespace Restaurant_API.Exceptions
{
	public class UnauthorizedException : HttpException
	{
		public UnauthorizedException(string errorCode) : base(HttpStatusCode.Unauthorized, errorCode) { }
	}
}
