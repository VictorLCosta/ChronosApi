using System.Security.Claims;

using Application.Common.Extensions;
using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private ClaimsPrincipal? _user;
    private string? _userId;

    public string? Name => _user?.Identity?.Name;

    public string? GetUserId() =>
        TryResolveUserId() ?? _userId;

    public string? GetUserEmail() =>
        IsAuthenticated()
            ? _user!.GetEmail()
            : null;

    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) =>
        _user?.IsInRole(role) ?? false;

    public IEnumerable<Claim>? GetUserClaims() =>
        _user?.Claims;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null)
        {
            throw new InvalidOperationException("Current user was already initialized for this scope.");
        }

        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        if (!string.IsNullOrWhiteSpace(_userId))
        {
            throw new InvalidOperationException("Current user id was already initialized for this scope.");
        }

        if (!string.IsNullOrWhiteSpace(userId))
        {
            _userId = userId;
        }
    }

    private string? TryResolveUserId()
    {
        if (!IsAuthenticated())
        {
            return null;
        }

        return _user?.GetUserId();
    }
}