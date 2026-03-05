using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AppRoleClaim : IdentityRoleClaim<string>
{
    public string? CreatedBy { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
}