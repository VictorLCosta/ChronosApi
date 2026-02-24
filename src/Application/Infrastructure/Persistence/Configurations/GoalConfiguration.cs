using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Persistence.Configurations;

public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(g => g.Notes)
            .HasMaxLength(3000);

        builder.Property(g => g.Status)
            .HasConversion<int>()
            .HasDefaultValue(0); // NotStarted

        builder.Property(g => g.Priority)
            .HasConversion<int>()
            .HasDefaultValue(1); // Medium

        // Value Object - RecurrenceRule
        builder.ComplexProperty(g => g.RecurrenceRule);

        // Relationships
        builder.HasOne(g => g.Project)
            .WithMany(p => p.Goals)
            .HasForeignKey(g => g.ProjectId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(g => g.Tasks)
            .WithOne(t => t.Goal)
            .HasForeignKey(t => t.GoalId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Many-to-Many relationship with Tags
        builder.HasMany(g => g.Tags)
            .WithMany(tg => tg.Goals)
            .UsingEntity(j => j.ToTable("GoalTags"));

        // Indexes
        builder.HasIndex(g => g.ProjectId);
        builder.HasIndex(g => g.Status);
        builder.HasIndex(g => g.Priority);
        builder.HasIndex(g => g.Created);

        // Full-Text Search Index
        builder.HasGeneratedTsVectorColumn(
            t => t.SearchVector,
            "english",
            t => new { t.Title, t.Notes })
            .HasIndex(t => t.SearchVector)
            .HasMethod("GIN");
    }
}