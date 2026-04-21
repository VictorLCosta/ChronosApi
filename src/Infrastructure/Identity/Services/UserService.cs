using System.Text;

using Application.Common.Identity.Models;
using Application.Common.Identity.Services;

using Ardalis.Result;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Identity.Services;

public class UserService(UserManager<AppUser> userManager, ILogger<UserService> logger) : IUserService
{
    public async Task<Result<string>> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Where(x => x.Id == userId && x.EmailConfirmed == false)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return Result.Error("n error occurred while confirming E-Mail");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            logger.LogInformation("E-Mail for user {UserId} confirmed successfully.", userId);
            return Result.Success("E-Mail confirmed successfully");
        }

        logger.LogError("An error occurred while confirming E-Mail for user {UserId}. Errors: {Errors}", userId, result.Errors);
        return Result.Error("An error occurred while confirming E-Mail");
    }

    public async Task<Result<string>> CreateAsync(string name, string username, string email, string password, string confirmPassword, string phoneNumber, string origin, CancellationToken cancellationToken)
    {
        if (password != confirmPassword)
            return Result.Error("Passwords do not match.");

        var user = new AppUser
        {
            Email = email,
            UserName = username,
            Name = name,
            PhoneNumber = phoneNumber,
            IsActive = true,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description).ToList();
            return Result.Error(new ErrorList(errors));
        }

        return user.Id;
    }

    public async Task<Result> DeleteAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Error("User not found.");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description).ToList();
            return Result.Error(new ErrorList(errors));
        }

        return Result.NoContent();
    }

    public async Task<UserDto?> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null) return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
        };
    }

    private async Task<string> GetEmailVerificationUriAsync(AppUser user, string origin)
    {
        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        const string route = "api/identity/confirm-email";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));

        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userid", user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);

        return verificationUri;
    }
}