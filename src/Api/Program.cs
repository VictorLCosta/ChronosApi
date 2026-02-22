using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

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

app.UseHttpsRedirection();

app.Run();
