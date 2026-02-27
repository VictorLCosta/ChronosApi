using System.Reflection;

using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions options)
    : BaseDbContext(options), IApplicationDbContext
{
    public DbSet<Attachment> Attachments => Set<Attachment>();

    public DbSet<Goal> Goals => Set<Goal>();

    public DbSet<GoalLog> GoalLogs => Set<GoalLog>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Reminder> Reminders => Set<Reminder>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}