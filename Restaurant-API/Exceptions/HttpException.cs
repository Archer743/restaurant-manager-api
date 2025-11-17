using System.Net;

namespace Restaurant_API.Exceptions
{
	public class HttpException : Exception
	{
		public HttpException(HttpStatusCode statusCode, string errorCode) : base()
		{
			StatusCode = statusCode;  // Response Status Code
			ErrorCode = errorCode;    // Provides more detailed information about the error for frontend use.
		}

		public HttpStatusCode StatusCode { get; private set; }

		public string ErrorCode { get; private set; }  // All error codes are defined in Constants/ErrorCodes.cs
	}
}
