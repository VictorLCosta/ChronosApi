using System.Net;

namespace CrossCutting.Exceptions;

public class AppException(string message, HttpStatusCode statusCode, Exception? innerException = null) : Exception(message, innerException)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public AppException()
        : this("An error occurred", HttpStatusCode.InternalServerError)
    {
    }

    public AppException(string message) : this(message, HttpStatusCode.InternalServerError)
    {
    }

    public AppException(string message, Exception innerException) : this(message, HttpStatusCode.InternalServerError, innerException)
    {
    }

}