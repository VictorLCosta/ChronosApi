namespace Application.Common.Interfaces;

public interface IRequestContext
{
    string? ClientId { get; }
    string? IpAddress { get; }
    string? Origin { get; }
    string? UserAgent { get; }
}