namespace Application.Common.Identity.Models;

public class UserDto
{
    public string? Id { get; set; }

    public string? UserName { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public bool EmailConfirmed { get; set; }

    public string? AvatarUrl { get; set; }
}