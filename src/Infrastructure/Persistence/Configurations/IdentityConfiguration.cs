using Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ApplicationUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("Users", "identity");

        builder
            .Property(u => u.ObjectId)
                .HasMaxLength(256);

        builder
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ApplicationRoleConfig : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("Roles", "identity");
    }
}

public class ApplicationRoleClaimConfig : IEntityTypeConfiguration<AppRoleClaim>
{
    public void Configure(EntityTypeBuilder<AppRoleClaim> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("RoleClaims", "identity");
    }
}

public class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("UserRoles", "identity");
    }
}

public class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("UserClaims", "identity");
    }
}

public class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("UserLogins", "identity");
    }
}

public class IdentityUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("UserTokens", "identity");
    }
}

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToTable("RefreshTokens", "identity");

        builder
            .HasKey(rt => rt.Id);

        builder
            .Property(rt => rt.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder
            .Property(rt => rt.TokenHash)
            .IsRequired()
            .HasMaxLength(512);

        builder
            .Property(rt => rt.FamilyId)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(rt => rt.RevokedReason)
            .HasMaxLength(500);

        builder
            .Property(rt => rt.ReplacedByTokenHash)
            .HasMaxLength(512);

        builder
            .Property(rt => rt.IpAddress)
            .HasMaxLength(64);

        builder
            .Property(rt => rt.UserAgent)
            .HasMaxLength(1024);

        builder
            .HasIndex(rt => rt.TokenHash)
            .IsUnique();

        builder
            .HasIndex(rt => new { rt.UserId, rt.ExpiresAtUtc });

        builder
            .HasIndex(rt => new { rt.UserId, rt.RevokedAtUtc });
    }
}