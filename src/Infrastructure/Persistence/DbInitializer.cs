using Application.Common.Interfaces;

using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

using Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DbInitializer(
    ApplicationDbContext context,
    RoleManager<AppRole> roleManager,
    UserManager<AppUser> userManager) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedUsersAsync(cancellationToken);

        // Seed some sample domain data if none exists
        if (!await context.Projects.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            var sampleTag = new Tag { Name = "sample" };

            var sampleProject = new Project
            {
                Title = "Sample Project"
            };

            var sampleGoal = new Goal
            {
                Title = "Sample Goal",
                Notes = "This is a seeded goal for testing.",
                Priority = PriorityLevel.Medium,
                Status = GoalStatus.Active,
                Project = sampleProject,
                RecurrenceRule = new RecurrenceRule
                {
                    Frequency = RecurrenceFrequency.Daily,
                    Interval = 1,
                    DaysOfWeek = [DayOfWeek.Friday],
                    StartsAt = DateTime.UtcNow,
                    EndsAt = DateTime.UtcNow.AddMonths(1)
                }
            };

            var task1 = new TaskItem
            {
                Title = "Sample Task 1",
                Notes = "First seeded task",
                Project = sampleProject,
                DueDate = DateTime.UtcNow.AddDays(7),
                RecurrenceRule = new RecurrenceRule
                {
                    Frequency = RecurrenceFrequency.Weekly,
                    Interval = 1,
                    DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday],
                    StartsAt = DateTime.UtcNow,
                    EndsAt = DateTime.UtcNow.AddMonths(1)
                }
            };

            var task2 = new TaskItem
            {
                Title = "Sample Task 2",
                Notes = "Second seeded task",
                Project = sampleProject,
                Goal = sampleGoal,
                DueDate = DateTime.UtcNow.AddDays(14),
                RecurrenceRule = new RecurrenceRule
                {
                    Frequency = RecurrenceFrequency.Once,
                    Interval = 1,
                    DaysOfWeek = [DayOfWeek.Monday],
                    StartsAt = DateTime.UtcNow,
                    EndsAt = DateTime.UtcNow.AddMonths(1)
                }
            };

            task1.Tags.Add(sampleTag);
            sampleGoal.Tags.Add(sampleTag);

            context.AddRange(sampleTag, sampleProject, sampleGoal, task1, task2);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        const string adminRoleName = "Admin";
        const string userRoleName = "User";

        if (!await roleManager.RoleExistsAsync(adminRoleName).ConfigureAwait(false))
        {
            await roleManager.CreateAsync(new AppRole(adminRoleName, "Administrator role")).ConfigureAwait(false);
        }

        if (!await roleManager.RoleExistsAsync(userRoleName).ConfigureAwait(false))
        {
            await roleManager.CreateAsync(new AppRole(userRoleName, "Basic user role")).ConfigureAwait(false);
        }

        // Create admin user
        var adminEmail = "admin@example.com";
        var admin = await userManager.FindByEmailAsync(adminEmail).ConfigureAwait(false);
        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                IsActive = true,
                Name = "Administrator"
            };

            var result = await userManager.CreateAsync(admin, "Admin123!").ConfigureAwait(false);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, adminRoleName).ConfigureAwait(false);
            }
        }

        // Create basic user
        var userEmail = "user@example.com";
        var basic = await userManager.FindByEmailAsync(userEmail).ConfigureAwait(false);
        if (basic == null)
        {
            basic = new AppUser
            {
                UserName = "user",
                Email = userEmail,
                EmailConfirmed = true,
                IsActive = true,
                Name = "Basic User"
            };

            var result = await userManager.CreateAsync(basic, "User123!").ConfigureAwait(false);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(basic, userRoleName).ConfigureAwait(false);
            }
        }
    }
}