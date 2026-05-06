using System.Security.Claims;

namespace Application.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Email);

    public static string? GetUserId(this ClaimsPrincipal principal) =>
            principal?.FindFirstValue(ClaimTypes.NameIdentifier);

    private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
        principal?.FindFirst(claimType)?.Value;
}