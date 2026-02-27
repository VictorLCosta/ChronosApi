namespace Application.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

    public Guid? CreatedBy { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public Guid? LastModifiedBy { get; set; }
}