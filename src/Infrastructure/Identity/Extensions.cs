using Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public static class Extensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddIdentity()
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 9;
                options.User.RequireUniqueEmail = true;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }
}
