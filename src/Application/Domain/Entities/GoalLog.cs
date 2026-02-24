namespace Application.Domain.Entities;

public class GoalLog : BaseEntity
{
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }

    public bool? Completed { get; set; }

    public Guid GoalId { get; set; }
    public Goal Goal { get; set; } = null!;
}
