using Application.Interfaces;
using Contracts;
using Contracts.Requests.User;
using Contracts.Responses.User;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;
public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // GET all Users
        app.MapGet(ApiEndpoints.User.GetAll, async ([FromServices] IUserService<UserResponse> us
            ) => await us.GetAll())
            .WithName("GetAllUsers").Produces<List<UserResponse>>();

        // GET User by ID
        app.MapGet(ApiEndpoints.User.GetById, async (int id, [FromServices] IUserService<UserResponse> us
            ) => await us.GetById(id))
            .WithName("GetUserById").Produces<UserResponse?>();

        // POST User
        app.MapPost(ApiEndpoints.User.Create, async (CreateUserRequest createRequest, [FromServices] IUserService<UserResponse> us
            ) =>
        await us.Create(createRequest)).WithName("CreateUser").Produces<UserResponse>();

        // UPDATE User
        app.MapPut(ApiEndpoints.User.Update, async (UpdateUserRequest updateRequest, [FromServices] IUserService<UserResponse> us
            ) =>
        await us.Update(updateRequest)).WithName("UpdateUser").Produces<UserResponse>();

        // DELETE User
        app.MapDelete(ApiEndpoints.User.Delete, async (int id, [FromServices] IUserService<UserResponse> us
            ) => await us.Delete(id))
            .WithName("DeleteUser").Produces<string>();
    }
}
