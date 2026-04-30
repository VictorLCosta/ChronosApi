using Api;

using Application;

using Infrastructure;
using Infrastructure.Logging;

using Scalar.AspNetCore;

using Serilog;

StaticLogger.EnsureInitialized();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddApplication();
    builder.AddInfrastructure();

    var app = builder.Build();

    app.UseInfrastructure();
    app.MapApiEndpoints();

    await app.Services.InitializeDatabasesAsync();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}
