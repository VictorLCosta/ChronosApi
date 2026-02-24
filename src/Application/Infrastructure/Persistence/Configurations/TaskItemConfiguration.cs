using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Notes)
            .HasMaxLength(3000);

        builder.Property(t => t.DueDate)
            .IsRequired(false);

        builder.Property(t => t.StartDate)
            .IsRequired(false);

        // Value Object - RecurrenceRule
        builder.ComplexProperty(g => g.RecurrenceRule);

        // Relationships
        builder.HasOne(t => t.Goal)
            .WithMany(g => g.Tasks)
            .HasForeignKey(t => t.GoalId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.TaskItems)
            .HasForeignKey(t => t.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.ParentTask)
            .WithMany(t => t.SubTasks)
            .HasForeignKey(t => t.ParentTaskId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-Many relationship with Tags
        builder.HasMany(t => t.Tags)
            .WithMany(tg => tg.TaskItems)
            .UsingEntity(j => j.ToTable("TaskItemTags"));

        builder.HasMany(t => t.Reminders)
            .WithOne(r => r.TaskItem)
            .HasForeignKey(r => r.TaskItemId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.GoalId);
        builder.HasIndex(t => t.ParentTaskId);
        builder.HasIndex(t => t.DueDate);

        // Full-Text Search Index
        builder.HasGeneratedTsVectorColumn(
            t => t.SearchVector,
            "english",
            t => new { t.Title, t.Notes })
            .HasIndex(t => t.SearchVector)
            .HasMethod("GIN");
    }
}