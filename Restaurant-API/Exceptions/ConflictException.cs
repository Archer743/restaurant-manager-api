using System.Net;

namespace Restaurant_API.Exceptions
{
	public class ConflictException : HttpException
	{
		public ConflictException(string errorCode) : base(HttpStatusCode.Conflict, errorCode) { }
	}
}
