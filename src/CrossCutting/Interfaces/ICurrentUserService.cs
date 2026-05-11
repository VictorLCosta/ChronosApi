using System.Security.Claims;

namespace CrossCutting.Interfaces;

public interface ICurrentUserService
{
    string? Name { get; }

    string? GetUserId();

    string? GetUserEmail();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();

    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}