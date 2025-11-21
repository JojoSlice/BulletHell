using Api.Endpoints;
using Application.ServiceCollection;
using Repository.ServiceCollection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRepositoryServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapHighScoreEndpoints();
app.MapUserEndpoints();

app.Run();
