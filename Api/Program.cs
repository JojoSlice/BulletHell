using Api.Endpoints;
using Application.ServiceCollection;
using Repository.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices();
builder.Services.AddDbContext<MyDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapHighScoreEndpoints();
app.MapUserEndpoints();

app.Run();

// Needed for WebApplicationFactory in tests
public partial class Program { }
