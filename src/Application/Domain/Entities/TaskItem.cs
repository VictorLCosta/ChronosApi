using Application.Domain.ValueObjects;

namespace Application.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }

    public RecurrenceRule? Recurrence { get; set; }
    public ICollection<Reminder> Reminders { get; set; } = [];

    public Guid? GoalId { get; set; }
    public Goal? Goal { get; set; }

    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }

    public Guid? ParentTaskId { get; set; } // for parent-subtask relationship
    public TaskItem? ParentTask { get; set; }
    public ICollection<TaskItem> SubTasks { get; set; } = [];

    public ICollection<Tag> Tags { get; set; } = [];
}