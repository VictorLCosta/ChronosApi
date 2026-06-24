using Application.Features.Projects;

using Domain.Entities;

using UnitTests.Common.Fixtures;

namespace UnitTests.Features.Projects.Queries;

internal sealed class SearchAllProjectsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyProjectsOwnedByCurrentUser()
    {
        const string currentUserId = "user-123";
        var includedProjectId = Guid.NewGuid();

        await using var fixture = new QueryHandlerFixture();
        fixture.CurrentUser.UserId = currentUserId;

        await SeedProjectAsync(fixture, includedProjectId, "Projeto do usuario", currentUserId, DateTimeOffset.UtcNow);
        await SeedProjectAsync(fixture, Guid.NewGuid(), "Projeto de outro usuario", "user-456", DateTimeOffset.UtcNow.AddMinutes(-5));

        var sut = CreateSut(fixture);

        var result = await sut.Handle(
            new SearchAllProjectsQuery
            {
                PageNumber = 1,
                PageSize = 10
            },
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.TotalCount.ShouldBe(1);
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Items.Single().Id.ShouldBe(includedProjectId);
    }

    [Fact]
    public async Task Handle_ShouldApplyExplicitSorting_WhenSortIsProvided()
    {
        const string currentUserId = "user-123";

        await using var fixture = new QueryHandlerFixture();
        fixture.CurrentUser.UserId = currentUserId;

        await SeedProjectAsync(fixture, Guid.NewGuid(), "Zulu", currentUserId, DateTimeOffset.UtcNow.AddMinutes(-10));
        await SeedProjectAsync(fixture, Guid.NewGuid(), "Alpha", currentUserId, DateTimeOffset.UtcNow);

        var sut = CreateSut(fixture);

        var result = await sut.Handle(
            new SearchAllProjectsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Sort = "Title asc"
            },
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Items.Select(project => project.Title).ShouldBe(["Alpha", "Zulu"]);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPage_WhenCurrentUserHasNoProjects()
    {
        await using var fixture = new QueryHandlerFixture();
        fixture.CurrentUser.UserId = "user-123";

        var sut = CreateSut(fixture);

        var result = await sut.Handle(
            new SearchAllProjectsQuery
            {
                PageNumber = 1,
                PageSize = 10
            },
            CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.TotalCount.ShouldBe(0);
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalPages.ShouldBe(0);
    }

    private static SearchAllProjectsQueryHandler CreateSut(QueryHandlerFixture fixture)
    {
        return new SearchAllProjectsQueryHandler(fixture.Context, fixture.CurrentUser);
    }

    private static async Task SeedProjectAsync(
        QueryHandlerFixture fixture,
        Guid id,
        string title,
        string createdBy,
        DateTimeOffset created)
    {
        fixture.Context.Projects.Add(new Project
        {
            Id = id,
            Title = title,
            CreatedBy = createdBy,
            Created = created
        });

        await fixture.Context.SaveChangesAsync(CancellationToken.None);
    }
}