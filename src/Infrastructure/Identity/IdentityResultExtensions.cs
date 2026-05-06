using System.Collections.ObjectModel;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static ReadOnlyCollection<string> GetErrors(this IdentityResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return [.. result.Errors.Select(e => e.Description.ToString())];
    }
}