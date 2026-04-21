using System.Net;

namespace Application.Common.Exceptions;

public sealed class UnauthorizedException(string message = "Invalid credentials or session.")
    : AppException(message, HttpStatusCode.Unauthorized);