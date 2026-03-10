using System.Net;

namespace Application.Common.Exceptions;

public class AppException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
