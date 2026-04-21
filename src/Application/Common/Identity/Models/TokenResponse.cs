namespace Application.Common.Identity.Models;

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    DateTime AccessTokenExpiresAt
);