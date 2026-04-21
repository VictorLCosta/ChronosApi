namespace Domain.ValueObjects;

public class DeviceInfo : ValueObject
{
    public string? DeviceName { get; set; }           // e.g., "iPhone 14", "Windows-PC-123"
    public string? BrowserName { get; set; }          // e.g., "Chrome", "Safari", "Firefox"
    public string? BrowserVersion { get; set; }       // e.g., "120.0.0"
    public string? OperatingSystem { get; set; }      // e.g., "Windows 11", "iOS 17"
    public string? OSVersion { get; set; }            // e.g., "10.0"
    public string? UserAgent { get; set; }

    public DeviceInfo() { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DeviceName ?? string.Empty;
        yield return BrowserName ?? string.Empty;
        yield return BrowserVersion ?? string.Empty;
        yield return OperatingSystem ?? string.Empty;
        yield return OSVersion ?? string.Empty;
        yield return UserAgent ?? string.Empty;
    }
}