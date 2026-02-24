using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Persistence.Configurations;

public class GoalLogConfiguration : IEntityTypeConfiguration<GoalLog>
{
    public void Configure(EntityTypeBuilder<GoalLog> builder)
    {
        builder.HasKey(gl => gl.Id);

        builder.Property(gl => gl.Date)
            .IsRequired();

        builder.Property(gl => gl.Notes)
            .HasMaxLength(2000);

        builder.Property(gl => gl.Completed)
            .IsRequired(false);

        // Relationships
        builder.HasOne(gl => gl.Goal)
            .WithMany()
            .HasForeignKey(gl => gl.GoalId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(gl => gl.GoalId);
        builder.HasIndex(gl => gl.Date);
        builder.HasIndex(g => new { g.GoalId, g.Date }).IsUnique();
    }
}