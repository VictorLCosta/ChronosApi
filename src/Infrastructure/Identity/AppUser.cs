using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string? Name { get; set; }

    public string? ObjectId { get; set; }

    public Uri? AvatarUrl { get; set; }
    public bool IsActive { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}