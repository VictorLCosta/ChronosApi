namespace Domain.Contracts;

public interface ITrashable
{
    bool IsTrashed { get; set; }
    DateTimeOffset? TrashedOnUtc { get; set; }
    Guid? TrashedBy { get; set; }
}