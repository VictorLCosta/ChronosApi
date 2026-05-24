using Domain.Contracts;
using Domain.Enums;
using Domain.ValueObjects;

using NpgsqlTypes;

namespace Domain.Entities;

public class Goal : BaseEntity, ITrashable
{
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<TaskItem> Tasks { get; init; } = [];
    public ICollection<Tag> Tags { get; init; } = [];

    public RecurrenceRule RecurrenceRule { get; set; } = null!;

    public bool IsTrashed { get; set; }
    public DateTimeOffset? TrashedOnUtc { get; set; }
    public Guid? TrashedBy { get; set; }

    public NpgsqlTsVector SearchVector { get; set; } = default!;
}