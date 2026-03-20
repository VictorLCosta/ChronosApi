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

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    builder.AddApplication();
    builder.AddInfrastructure();

    var app = builder.Build();

    app.MapApiEndpoints();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference("api-docs", (opt) =>
        {
            opt.Title = "Scalar API Reference";
            opt.DarkMode = true;
            opt.Theme = ScalarTheme.BluePlanet;
            opt.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            opt.ShowSidebar = true;
        });
    }

    app.UseInfrastructure();

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