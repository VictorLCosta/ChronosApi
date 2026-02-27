using Application.Domain.Enums;
using Application.Domain.ValueObjects;

using NpgsqlTypes;

namespace Domain.Entities;

public class Goal : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];

    public RecurrenceRule RecurrenceRule { get; set; } = null!;

    public NpgsqlTsVector SearchVector { get; set; } = default!;
}