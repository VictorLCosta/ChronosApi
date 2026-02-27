using System.Security.Claims;

using Application.Common.Extensions;
using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    #region Fields

    private ClaimsPrincipal? _user;

    private Guid _userId = Guid.Empty;

    #endregion

    #region Properties

    public string? Name => _user?.Identity?.Name;

    #endregion

    #region Current User Info

    public Guid GetUserId() =>
        IsAuthenticated()
            ? Guid.Parse(_user?.GetUserId() ?? Guid.Empty.ToString())
            : _userId;

    public string? GetUserEmail() =>
        IsAuthenticated()
            ? _user!.GetEmail()
            : string.Empty;

    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role)
    {
        throw new NotImplementedException();
    }
    
    public IEnumerable<Claim>? GetUserClaims() =>
        _user?.Claims;

    #endregion

    #region Methods

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        _user = user;
    }

    public void SetCurrentUserId(Guid userId)
    {
        if (_userId != Guid.Empty)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        if (userId != Guid.Empty)
        {
            _userId = userId;
        }
    }

    #endregion
}
