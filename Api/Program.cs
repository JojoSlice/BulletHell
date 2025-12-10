using Api.Endpoints;
using Application.ServiceCollection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Ensure database exists
app.Services.EnsureDatabaseCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapHighScoreEndpoints();
app.MapUserEndpoints();

app.Run();

// Needed for WebApplicationFactory in tests
public partial class Program { }
