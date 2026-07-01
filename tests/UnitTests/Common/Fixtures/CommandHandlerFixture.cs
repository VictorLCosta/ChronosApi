using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

using UnitTests.Common.Helpers;

namespace UnitTests.Common.Fixtures;

internal sealed class CommandHandlerFixture : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public TestApplicationDbContext Context { get; }
    public FakeCurrentUserService CurrentUser { get; } = new();

    public CommandHandlerFixture()
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
        await _connection.DisposeAsync();
        await Context.DisposeAsync();
    }
}
