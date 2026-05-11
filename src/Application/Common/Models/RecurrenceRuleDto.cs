using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Common.Models;

public sealed record RecurrenceRuleDto(
    RecurrenceFrequency Frequency = RecurrenceFrequency.Once,
    int Interval = 1,
    IReadOnlyCollection<DayOfWeek>? DaysOfWeek = null,
    int? DayOfMonth = null,
    int? MonthOfYear = null,
    DateTime? StartsAt = null,
    DateTime? EndsAt = null,
    int? OccurrenceCount = null
);

public static class RecurrenceRuleMappings
{
    public static RecurrenceRule ToValueObject(this RecurrenceRuleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new RecurrenceRule
        {
            Frequency = dto.Frequency,
            Interval = dto.Interval,
            DaysOfWeek = dto.DaysOfWeek ?? [],
            DayOfMonth = dto.DayOfMonth,
            MonthOfYear = dto.MonthOfYear,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            OccurrenceCount = dto.OccurrenceCount
        };
    }

    public static RecurrenceRuleDto? ToDto(this RecurrenceRule? recurrenceRule)
    {
        if (recurrenceRule is null)
            return null;

        return new RecurrenceRuleDto(
            recurrenceRule.Frequency,
            recurrenceRule.Interval,
            recurrenceRule.DaysOfWeek,
            recurrenceRule.DayOfMonth,
            recurrenceRule.MonthOfYear,
            recurrenceRule.StartsAt,
            recurrenceRule.EndsAt,
            recurrenceRule.OccurrenceCount
        );
    }
}
