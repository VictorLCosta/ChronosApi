using Application.Features.Projects;

using Domain.Entities;

using UnitTests.Common.Fixtures;

namespace UnitTests.Features.Projects.Queries;

public sealed class GetProjectByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnProjectDto_WhenProjectBelongsToCurrentUser()
    {
        const string userId = "user-123";
        var projectId = Guid.NewGuid();

        await using var fixture = new QueryHandlerFixture();
        fixture.CurrentUser.UserId = userId;
        await AddProjectAsync(fixture, projectId, "Projeto Alpha", userId);

        var sut = CreateSut(fixture);

        var result = await sut.Handle(new GetProjectByIdQuery(projectId), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(projectId);
        result.Value.Title.ShouldBe("Projeto Alpha");
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenProjectBelongsToAnotherUser()
    {
        const string currentUserId = "user-123";
        const string ownerUserId = "user-456";
        var projectId = Guid.NewGuid();

        await using var fixture = new QueryHandlerFixture();
        fixture.CurrentUser.UserId = currentUserId;
        await AddProjectAsync(fixture, projectId, "Projeto Restrito", ownerUserId);

        var sut = CreateSut(fixture);

        var result = await sut.Handle(new GetProjectByIdQuery(projectId), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        await using var fixture = new QueryHandlerFixture();
        fixture.CurrentUser.UserId = "user-123";

        var sut = CreateSut(fixture);

        var result = await sut.Handle(new GetProjectByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Status.ShouldBe(Ardalis.Result.ResultStatus.NotFound);
        result.Value.ShouldBeNull();
    }

    private static GetProjectByIdQueryHandler CreateSut(QueryHandlerFixture fixture)
    {
        return new GetProjectByIdQueryHandler(fixture.Context, fixture.CurrentUser);
    }

    private static async Task AddProjectAsync(
        QueryHandlerFixture fixture,
        Guid projectId,
        string title,
        string createdBy)
    {
        fixture.Context.Projects.Add(new Project
        {
            Id = projectId,
            Title = title,
            CreatedBy = createdBy
        });

        await fixture.Context.SaveChangesAsync(CancellationToken.None);
    }
}