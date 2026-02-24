using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Attachment> Attachments { get; }
    DbSet<Goal> Goals { get; }
    DbSet<GoalLog> GoalLogs { get; }
    DbSet<Project> Projects { get; }
    DbSet<Reminder> Reminders { get; }
    DbSet<Tag> Tags { get; }
    DbSet<TaskItem> Tasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}