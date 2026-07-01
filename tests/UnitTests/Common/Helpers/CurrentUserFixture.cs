using System.Security.Claims;

using CrossCutting.Interfaces;

namespace UnitTests.Common.Helpers;

internal sealed class FakeCurrentUserService : ICurrentUserService
{
    private ClaimsPrincipal? _user;

    public string? Name => _user?.Identity?.Name;

    public string? UserId { get; set; } = "user-123";

    public string? Email { get; set; }

    public bool Authenticated { get; set; }

    public IEnumerable<Claim>? Claims => _user?.Claims;

    public string? GetUserId() => UserId;

    public string? GetUserEmail() => Email;

    public bool IsAuthenticated() => Authenticated || !string.IsNullOrWhiteSpace(UserId);

    public bool IsInRole(string role) => _user?.IsInRole(role) ?? false;

    public IEnumerable<Claim>? GetUserClaims() => _user?.Claims;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        UserId = userId;
    }
}