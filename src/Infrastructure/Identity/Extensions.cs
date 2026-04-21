using Application.Common.Identity.Services;
using Application.Common.Interfaces;

using Infrastructure.Identity.Services;
using Infrastructure.Persistence;
using Infrastructure.Services;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAppIdentity()
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IRequestContext, RequestContext>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            services.AddScoped<IUserPasswordService, UserPasswordService>();
            services.AddSingleton(TimeProvider.System);

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;

                options.Lockout.MaxFailedAccessAttempts = 3;

                options.SignIn.RequireConfirmedEmail = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }
}