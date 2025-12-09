using Api.Endpoints;
using Application.ServiceCollection;
using Microsoft.EntityFrameworkCore;
using Repository.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices();
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlite($"Data Source={MyDbContext.GetDefaultDatabasePath()}"));

var app = builder.Build();

var dbPath = MyDbContext.GetDefaultDatabasePath();
if (!File.Exists(dbPath))
{
    // Skapa en scope för att få DbContext från DI
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

    // Om du använder EF-migrationer, använd:
    // db.Database.Migrate();

    // Annars: skapa databasen baserat på modellen
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapHighScoreEndpoints();
app.MapUserEndpoints();

app.Run();

// Needed for WebApplicationFactory in tests
public partial class Program { }
