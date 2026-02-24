using Application.Domain.Enums;
using Application.Domain.ValueObjects;

namespace Application.Domain.Entities;

public class Goal : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];

    public RecurrenceRule RecurrenceRule { get; set; } = null!;
}
