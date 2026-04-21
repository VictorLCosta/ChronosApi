using System.Security.Claims;

using Application.Common.Identity.Models;

namespace Application.Common.Identity.Services;

public interface ITokenService
{
    Task<TokenResponse> GetTokenAsync(string subject, IEnumerable<Claim> claims, CancellationToken cancellationToken);
}