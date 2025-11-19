using Application.Interfaces;
using Application.ServiceCollection;
using Repository.ServiceCollection;
using Contracts;
using Contracts.Responses.HighScore;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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
app.MapGet(ApiEndpoints.HighScore.GetAll, async ([FromServices] IService<HighScoreResponse, HighScore> hss) => 
    await hss.GetAll()).WithName("GetAllHighScores").Produces<List<HighScoreResponse>>(statusCode: StatusCodes.Status200OK);

app.Run();