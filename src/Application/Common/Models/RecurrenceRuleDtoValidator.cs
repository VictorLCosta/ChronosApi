using Domain.Enums;

using FluentValidation;

namespace Application.Common.Models;

public class RecurrenceRuleDtoValidator : AbstractValidator<RecurrenceRuleDto>
{
    public RecurrenceRuleDtoValidator()
    {
        RuleFor(x => x.Interval)
            .GreaterThan(0)
            .WithMessage("Recurrence interval must be greater than zero");

        RuleFor(x => x.DayOfMonth)
            .InclusiveBetween(1, 31)
            .When(x => x.DayOfMonth.HasValue)
            .WithMessage("Recurrence day of month must be between 1 and 31");

        RuleFor(x => x.MonthOfYear)
            .InclusiveBetween(1, 12)
            .When(x => x.MonthOfYear.HasValue)
            .WithMessage("Recurrence month of year must be between 1 and 12");

        RuleFor(x => x.OccurrenceCount)
            .GreaterThan(0)
            .When(x => x.OccurrenceCount.HasValue)
            .WithMessage("Recurrence occurrence count must be greater than zero");

        RuleFor(x => x)
            .Must(x => !x.StartsAt.HasValue || !x.EndsAt.HasValue || x.StartsAt <= x.EndsAt)
            .WithMessage("Recurrence start date must be before end date");

        RuleFor(x => x.DaysOfWeek)
            .Must(days => days is { Count: > 0 })
            .When(x => x.Frequency == RecurrenceFrequency.Weekly)
            .WithMessage("Weekly recurrence must include at least one day of week");

        RuleFor(x => x.DayOfMonth)
            .NotNull()
            .When(x => x.Frequency == RecurrenceFrequency.Monthly)
            .WithMessage("Monthly recurrence must include a day of month");

        RuleFor(x => x.MonthOfYear)
            .NotNull()
            .When(x => x.Frequency == RecurrenceFrequency.Yearly)
            .WithMessage("Yearly recurrence must include a month of year");
    }
}