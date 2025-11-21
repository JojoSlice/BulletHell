using Application.Interfaces;
using Contracts;
using Contracts.Requests.HighScore;
using Contracts.Responses.HighScore;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;
public static class HighScoreEndpoints
{
    public static void MapHighScoreEndpoints(this WebApplication app)
    {
        // GET all HighScores
        app.MapGet(ApiEndpoints.HighScore.GetAll, async ([FromServices] IHighScoreService<HighScoreResponse> hss
            ) =>
        await hss.GetAll()).WithName("GetAllHighScores").Produces<List<HighScoreResponse>>();

        // GET HighScore by ID
        app.MapGet(ApiEndpoints.HighScore.GetById, async (int id, [FromServices] IHighScoreService<HighScoreResponse> hss
) =>
        await hss.GetById(id)).WithName("GetHighScoreById").Produces<HighScoreResponse?>();

        // POST HighScore
        app.MapPost(ApiEndpoints.HighScore.Create, async (CreateHighScoreRequest createRequest, [FromServices] IHighScoreService<HighScoreResponse> hss
            ) =>
        await hss.Create(createRequest)).WithName("CreateHighScore").Produces<HighScoreResponse>();

        // PUT HighScore
        app.MapPut(ApiEndpoints.HighScore.Update, async (UpdateHighScoreRequest updateRequest, [FromServices] IHighScoreService<HighScoreResponse> hss
            ) =>
        await hss.Update(updateRequest)).WithName("UpdateHighScore").Produces<HighScoreResponse>();

        // DELETE HighScore
        app.MapDelete(ApiEndpoints.HighScore.Delete, async (int id, [FromServices] IHighScoreService<HighScoreResponse> hss
            ) =>
        await hss.Delete(id)).WithName("DeleteHighScore").Produces<string>();
    }
}
