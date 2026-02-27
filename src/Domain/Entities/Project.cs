namespace Domain.Entities;

public class Project : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public ICollection<TaskItem> TaskItems { get; set; } = [];
    public ICollection<Goal> Goals { get; set; } = [];
}