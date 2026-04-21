namespace Application.Common.Identity.Models;

public sealed record RefreshTokenResponse(
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpiryTime
);