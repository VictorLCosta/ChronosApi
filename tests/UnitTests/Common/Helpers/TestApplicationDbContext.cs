using Application.Common.Interfaces;

using Domain.Common;
using Domain.Entities;
using Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NpgsqlTypes;

namespace UnitTests.Common.Helpers;

internal sealed class TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Attachment> Attachments => Set<Attachment>();

    public DbSet<Goal> Goals => Set<Goal>();

    public DbSet<GoalLog> GoalLogs => Set<GoalLog>();

    public DbSet<Reminder> Reminders => Set<Reminder>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(builder =>
        {
            ConfigureBaseEntity(builder);
            builder.HasKey(project => project.Id);
            builder.Property(project => project.Title).IsRequired();
            builder.Ignore(project => project.SearchVector);
        });

        modelBuilder
            .Entity<Goal>(builder =>
            {
                ConfigureBaseEntity(builder);
                builder.HasKey(goal => goal.Id);
                builder.Property(goal => goal.Title).IsRequired();
                builder.Property(goal => goal.Status).HasConversion<string>().HasDefaultValue(GoalStatus.NotStarted);
                builder.Property(goal => goal.Priority).HasConversion<string>().HasDefaultValue(PriorityLevel.Medium);
                builder.Ignore(goal => goal.SearchVector);
                builder.ComplexProperty(goal => goal.RecurrenceRule, ConfigureRecurrenceRule);
            });

        modelBuilder
            .Entity<TaskItem>(builder =>
            {
                ConfigureBaseEntity(builder);
                builder.HasKey(taskItem => taskItem.Id);
                builder.Property(taskItem => taskItem.Title).IsRequired();
                builder.Ignore(taskItem => taskItem.SearchVector);
                builder.ComplexProperty(taskItem => taskItem.RecurrenceRule, recurrenceRuleBuilder =>
                {
                    if (recurrenceRuleBuilder is not null)
                    {
                        ConfigureRecurrenceRule(recurrenceRuleBuilder);
                    }
                });
            });

        modelBuilder.Ignore<NpgsqlTsVector>();
    }

    private static void ConfigureBaseEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.Ignore(entity => entity.RowVersion);

        builder.Property(entity => entity.Created)
            .HasConversion(
                value => value.UtcDateTime,
                value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)));

        builder.Property(entity => entity.LastModified)
            .HasConversion(
                value => value.HasValue ? value.Value.UtcDateTime : (DateTime?)null,
                value => value.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : (DateTimeOffset?)null);
    }

    private static void ConfigureRecurrenceRule(ComplexPropertyBuilder<Domain.ValueObjects.RecurrenceRule> recurrenceRuleBuilder)
    {
        recurrenceRuleBuilder.Property(rule => rule.Frequency);
        recurrenceRuleBuilder.Property(rule => rule.Interval);
        recurrenceRuleBuilder.PrimitiveCollection(rule => rule.DaysOfWeek);
        recurrenceRuleBuilder.Property(rule => rule.DayOfMonth);
        recurrenceRuleBuilder.Property(rule => rule.MonthOfYear);
        recurrenceRuleBuilder.Property(rule => rule.StartsAt);
        recurrenceRuleBuilder.Property(rule => rule.EndsAt);
        recurrenceRuleBuilder.Property(rule => rule.OccurrenceCount);
    }
}