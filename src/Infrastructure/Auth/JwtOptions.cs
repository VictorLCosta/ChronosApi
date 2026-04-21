using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Auth;

public class JwtOptions
{
    [Required]
    [MinLength(32)]
    public string Key { get; init; } = default!;

    [Required]
    public string Issuer { get; init; } = default!;

    [Required]
    public string Audience { get; init; } = default!;

    [Range(1, double.MaxValue)]
    public double DurationInMinutes { get; init; }

    [Range(1, double.MaxValue)]
    public double RefreshTokenDurationInDays { get; init; }
}