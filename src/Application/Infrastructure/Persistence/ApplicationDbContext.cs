using Application.Common.Interfaces;
using Application.Domain.Entities;
using Application.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Persistence;

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

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
        modelBuilder.ApplyConfiguration(new GoalConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
        modelBuilder.ApplyConfiguration(new ReminderConfiguration());
        modelBuilder.ApplyConfiguration(new GoalLogConfiguration());
    }
}