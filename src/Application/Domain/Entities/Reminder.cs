namespace Application.Domain.Entities;

public class Reminder : BaseEntity
{
    public DateTime RemindAt { get; set; }
    public bool IsSent { get; set; } = false;

    // relative reminder (e.g., 15 minutes before) can be expressed with OffsetMinutes
    public int? OffsetMinutes { get; set; }

    public Guid? TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public Guid? GoalId { get; set; }
    public Goal? Goal { get; set; }
}