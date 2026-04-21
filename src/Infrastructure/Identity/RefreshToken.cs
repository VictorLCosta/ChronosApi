using Domain.Common;

namespace Infrastructure.Identity;

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public string FamilyId { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? LastUsedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? RevokedReason { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public AppUser? User { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsValid => !IsExpired && !IsRevoked;
}