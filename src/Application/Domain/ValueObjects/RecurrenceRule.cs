using Application.Domain.Enums;

namespace Application.Domain.ValueObjects;

public class RecurrenceRule : ValueObject
{
    public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.Once;
    public int Interval { get; set; } = 1; // every N units (e.g., every 2 days)
    public IReadOnlyCollection<DayOfWeek> DaysOfWeek { get; set; } = []; // e.g., "Mon,Wed,Fri" for weekly patterns
    public int? DayOfMonth { get; set; } // e.g., 15
    public int? MonthOfYear { get; set; } // e.g., 12
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public int? OccurrenceCount { get; set; } // end after N occurrences

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Frequency;
        yield return Interval;
        yield return DaysOfWeek;
        yield return DayOfMonth ?? 0;
        yield return MonthOfYear ?? 0;
        yield return StartsAt ?? DateTime.MinValue;
        yield return EndsAt ?? DateTime.MaxValue;
        yield return OccurrenceCount ?? 0;
    }
}
