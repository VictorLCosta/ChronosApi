using Application.Features.Goals;

using Bogus;

using Domain.Entities;
using Domain.ValueObjects;

using UnitTests.Common.Assertions;
using UnitTests.Common.Fixtures;

namespace UnitTests.Features.Goals.Queries;

public sealed class SearchAllGoalsQueryTests
{
    [Fact]
    public async Task ShouldReturn_EmptyListWhenNoGoalsExist()
    {
        await using var fixture = new QueryHandlerFixture();
        var sut = CreateSut(fixture);

        var result = await sut.Handle(new SearchAllGoalsQuery(), CancellationToken.None);
        var resultGoals = result.ShouldBeSuccess();

        resultGoals.ShouldBeEmptyPage();
    }

    [Fact]
    public async Task ShouldReturn_AllGoalsForCurrentUser()
    {
        await using var fixture = new QueryHandlerFixture();
        var sut = CreateSut(fixture);

        var goals = await AddGoalsAsync(fixture, fixture.CurrentUser.UserId!);
        var result = await sut.Handle(new SearchAllGoalsQuery(), CancellationToken.None);
        var resultGoals = result.ShouldBeSuccess();

        resultGoals.TotalCount.ShouldBe(goals.Count);
        resultGoals.Items.ShouldAllBe(g => goals.Any(x => x.Id == g.Id));
    }

    private static SearchAllGoalsQueryHandler CreateSut(QueryHandlerFixture fixture)
    {
        return new SearchAllGoalsQueryHandler(fixture.Context, fixture.CurrentUser);
    }

    private static async Task<List<Goal>> AddGoalsAsync(QueryHandlerFixture fixture, string userId)
    {
        var daysOfWeek = Enum
            .GetValues<DayOfWeek>()
            .ToList();

        var recurrenceRuleFaker = new Faker<RecurrenceRule>("pt_BR")
            .RuleFor(r => r.Frequency, f => f.PickRandom<Domain.Enums.RecurrenceFrequency>())
            .RuleFor(r => r.Interval, f => f.Random.Int(1, 10))
            .RuleFor(
                r => r.DaysOfWeek,
                f => [.. f.Random
                    .ListItems(daysOfWeek, f.Random.Int(1, 7))
                    .Distinct()])
            .RuleFor(r => r.DayOfMonth, f => f.Random.Bool() ? f.Random.Int(1, 28) : null)
            .RuleFor(r => r.MonthOfYear, f => f.Random.Bool() ? f.Random.Int(1, 12) : null)
            .RuleFor(r => r.StartsAt, f => f.Date.Recent())
            .RuleFor(r => r.EndsAt, (f, r) => f.Date.Soon(30, r.StartsAt ?? DateTime.UtcNow))
            .RuleFor(r => r.OccurrenceCount, f => f.Random.Bool() ? f.Random.Int(1, 20) : null);

        var faker = new Faker<Goal>("pt_BR")
            .RuleFor(g => g.Id, _ => Guid.NewGuid())
            .RuleFor(g => g.Title, f => f.Lorem.Sentence())
            .RuleFor(g => g.Notes, f => f.Lorem.Paragraph())
            .RuleFor(g => g.Status, f => f.PickRandom<Domain.Enums.GoalStatus>())
            .RuleFor(g => g.Priority, f => f.PickRandom<Domain.Enums.PriorityLevel>())
            .RuleFor(g => g.RecurrenceRule, f => recurrenceRuleFaker.Generate())
            .RuleFor(g => g.CreatedBy, _ => userId)
            .RuleFor(g => g.Created, f => f.Date.RecentOffset());

        var goals = faker.Generate(5);

        await fixture.Context.Goals.AddRangeAsync(goals);
        await fixture.Context.SaveChangesAsync();

        return goals;
    }
}