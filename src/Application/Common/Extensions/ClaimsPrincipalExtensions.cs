using System.Security.Claims;

namespace Application.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension (ClaimsPrincipal principal)
    {
        public string? GetEmail() =>
            principal.FindFirstValue(ClaimTypes.Email);

        public string? GetUserId() =>
            principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        private string? FindFirstValue(string claimType) =>
            principal?.FindFirst(claimType)?.Value;
    }
}
