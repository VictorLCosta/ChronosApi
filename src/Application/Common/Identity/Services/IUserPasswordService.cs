namespace Application.Common.Identity.Services;

public interface IUserPasswordService
{
    Task<Result<string>> ForgotPasswordAsync(string email, string origin, CancellationToken cancellationToken);
    Task<Result<string>> ResetPasswordAsync(string email, string password, string token, CancellationToken cancellationToken);
    Task<Result> ChangePasswordAsync(string password, string newPassword, string confirmNewPassword, string userId, CancellationToken cancellationToken);
}