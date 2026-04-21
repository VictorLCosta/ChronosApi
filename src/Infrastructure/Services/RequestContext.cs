using Application.Common.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Infrastructure.Services;

public sealed class RequestContext(IHttpContextAccessor httpContextAccessor) : IRequestContext
{
    private const string ClientIdHeader = "X-Client-Id";
    private const string ForwardedForHeader = "X-Forwarded-For";
    private const string OriginHeader = "Origin";

    public string? ClientId => GetHeaderValue(ClientIdHeader);

    public string? IpAddress
    {
        get
        {
            var forwardedFor = GetHeaderValue(ForwardedForHeader);
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor.Split(',', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }
    }

    public string? Origin
    {
        get
        {
            var origin = GetHeaderValue(OriginHeader);
            if (!string.IsNullOrWhiteSpace(origin))
            {
                return origin;
            }

            var request = httpContextAccessor.HttpContext?.Request;
            if (request is null || !request.Host.HasValue)
            {
                return null;
            }

            return $"{request.Scheme}://{request.Host}";
        }
    }

    public string? UserAgent => httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

    private string? GetHeaderValue(string headerName)
    {
        var headers = httpContextAccessor.HttpContext?.Request.Headers;
        if (headers is null || !headers.TryGetValue(headerName, out StringValues value))
        {
            return null;
        }

        return StringValues.IsNullOrEmpty(value)
            ? null
            : value.ToString();
    }
}