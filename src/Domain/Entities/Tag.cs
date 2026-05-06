namespace Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<TaskItem> TaskItems { get; init; } = [];
    public ICollection<Goal> Goals { get; init; } = [];
}