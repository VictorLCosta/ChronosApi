using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using UnitTests.Common.Helpers;

namespace UnitTests.Common.Fixtures;

internal sealed class QueryHandlerFixture : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public TestApplicationDbContext Context { get; }
    public FakeCurrentUserService CurrentUser { get; } = new();

    public QueryHandlerFixture()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new TestApplicationDbContext(options);
        Context.Database.EnsureCreated();
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}