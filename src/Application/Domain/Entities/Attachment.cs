namespace Application.Domain.Entities;

public class Attachment : BaseEntity
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long SizeBytes { get; set; }

    public string StorageUrl { get; set; } = default!;

    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
}