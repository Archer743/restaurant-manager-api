using Restaurant_API.Constants;
using System.Net;

namespace Restaurant_API.Exceptions
{
	public class ForbiddenException : HttpException
	{
		public ForbiddenException(string errorCode = ErrorCodes.Auth.ForbiddenAccess) : base(HttpStatusCode.Forbidden, errorCode) { }
	}
}
