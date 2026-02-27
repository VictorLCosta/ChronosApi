using Application.Common;

namespace Infrastructure.Audit;

public class AuditTrail : BaseEntity
{
    public Guid UserId { get; set; }
    public string? Operation { get; set; }
    public string? Entity { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string? PreviousValues { get; set; }
    public string? NewValues { get; set; }
    public string? ModifiedProperties { get; set; }
    public string? PrimaryKey { get; set; }
}
