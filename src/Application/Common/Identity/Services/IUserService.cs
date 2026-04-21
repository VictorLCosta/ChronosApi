using Application.Common.Identity.Models;

namespace Application.Common.Identity.Services;

public interface IUserService
{
    Task<Result<string>> CreateAsync(
        string name,
        string username,
        string email,
        string password,
        string confirmPassword,
        string phoneNumber,
        string origin,
        CancellationToken cancellationToken
    );

    Task<UserDto?> GetAsync(string userId, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(string userId);
    Task<Result<string>> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken);
}