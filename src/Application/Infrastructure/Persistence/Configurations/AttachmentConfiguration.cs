using Application.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Infrastructure.Persistence.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.SizeBytes)
            .IsRequired();

        builder.Property(a => a.StorageUrl)
            .IsRequired()
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(a => a.TaskItem)
            .WithMany()
            .HasForeignKey(a => a.TaskItemId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.TaskItemId);
        builder.HasIndex(a => a.Created);
    }
}