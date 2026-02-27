using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RemindAt)
            .IsRequired();

        builder.Property(r => r.IsSent)
            .HasDefaultValue(false);

        builder.Property(r => r.OffsetMinutes)
            .IsRequired(false);

        // Relationships
        builder.HasOne(r => r.TaskItem)
            .WithMany(t => t.Reminders)
            .HasForeignKey(r => r.TaskItemId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Goal)
            .WithMany()
            .HasForeignKey(r => r.GoalId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => r.TaskItemId);
        builder.HasIndex(r => r.GoalId);
        builder.HasIndex(r => r.RemindAt);
        builder.HasIndex(r => r.IsSent);
    }
}