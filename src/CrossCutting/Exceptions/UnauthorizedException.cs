using System.Net;

namespace CrossCutting.Exceptions;

public sealed class UnauthorizedException(string message = "Invalid credentials or session.")
    : AppException(message, HttpStatusCode.Unauthorized)
{
    public UnauthorizedException() : this("Invalid credentials or session.")
    {
    }

    public UnauthorizedException(string message, Exception innerException) : this(message)
    {
    }

}