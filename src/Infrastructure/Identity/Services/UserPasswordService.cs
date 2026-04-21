using System.Text;

using Application.Common.Identity.Services;
using Application.Common.Interfaces;

using Ardalis.Result;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Identity.Services;

public class UserPasswordService(
    UserManager<AppUser> userManager,
    IApplicationDbContext context) : IUserPasswordService
{
    public async Task<Result> ChangePasswordAsync(string password, string newPassword, string confirmNewPassword, string userId, CancellationToken cancellationToken)
    {
        if (!string.Equals(newPassword, confirmNewPassword, StringComparison.Ordinal))
        {
            return Result.Invalid([new ValidationError { ErrorMessage = "Passwords do not match." }]);
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user is null || !user.IsActive)
        {
            return Result.NotFound("User not found");
        }

        var result = await userManager.ChangePasswordAsync(user, password, newPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result.Invalid(errors.Select(e => new ValidationError { ErrorMessage = e }).ToList());
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<string>> ForgotPasswordAsync(string email, string origin, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !user.IsActive)
        {
            return Result.Success();
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return Result.Success();
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var resetPasswordUri = $"{origin}/reset-password?token={token}&email={email}";

        // TODO: Send email with resetPasswordUri

        return Result.Success("Password reset link has been sent to your email.");
    }

    public async Task<Result<string>> ResetPasswordAsync(string email, string password, string token, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !user.IsActive)
        {
            return Result.NotFound("User not found");
        }

        token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await userManager.ResetPasswordAsync(user, token, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result.Invalid(errors.Select(e => new ValidationError { ErrorMessage = e }).ToList());
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Password reset successful");
    }
}