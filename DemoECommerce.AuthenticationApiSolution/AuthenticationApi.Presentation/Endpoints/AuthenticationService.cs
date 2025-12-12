using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationApi.Presentation.Endpoints
{
    [AllowAnonymous]
    public static class AuthenticationService
    {
        public static void MapAuthenticationEndpoints(this WebApplication app) 
        {
            var groupAuth = app.MapGroup("/api/authentication")
                .WithTags("Authentication Services")
                .WithOpenApi();

            groupAuth.MapPost("/register", Register)
                .WithName("Register")
                .WithDescription("Registers a new user in the system.")
                .Accepts<AppUserDTO>("application/json")
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            groupAuth.MapPost("/login", Login)
                .WithName("Login")
                .WithDescription("Logs in a user and returns an authentication token.")
                .Accepts<LoginDTO>("application/json")
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            groupAuth.MapGet("/user/{userID}", GetUser)
                .WithName("GetUser")
                .WithDescription("Retrieves user information based on the provided user ID.")
                .Produces<GetUserDTO>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithOpenApi();
        }
        [Authorize]
        private static async Task<IResult> Register(IUser userService, AppUserDTO appUserDTO) 
        {
            var response = await userService.Register(appUserDTO);
            return response.Flag ? Results.Ok(response) : Results.BadRequest(response);
        }
        [AllowAnonymous]
        private static async Task<IResult> Login(IUser userService, LoginDTO loginDTO) 
        {
            var response = await userService.Login(loginDTO);
            return response.Flag ? Results.Ok(response) : Results.BadRequest(response);
        }
        [Authorize]
        private static async Task<IResult> GetUser(IUser userService, int userID) 
        {
            var response = await userService.GetUser(userID);
            return response is not null ? Results.Ok(response) : Results.NotFound();
        }
    }
}
