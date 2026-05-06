using NpgsqlTypes;

namespace Domain.Entities;

public class Project : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public ICollection<TaskItem> TaskItems { get; init; } = [];
    public ICollection<Goal> Goals { get; init; } = [];

    public NpgsqlTsVector SearchVector { get; set; } = default!;
}