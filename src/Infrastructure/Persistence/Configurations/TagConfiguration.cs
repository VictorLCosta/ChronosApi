using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Many-to-Many relationships
        builder.HasMany(t => t.TaskItems)
            .WithMany(ti => ti.Tags)
            .UsingEntity(j => j.ToTable("TaskItemTags"));

        builder.HasMany(t => t.Goals)
            .WithMany(g => g.Tags)
            .UsingEntity(j => j.ToTable("GoalTags"));

        // Indexes
        builder.HasIndex(t => t.Name).IsUnique();
    }
}