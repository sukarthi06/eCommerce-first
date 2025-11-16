
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string message = "Sorry internal server error occurred. Kindly try again";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Internal Server Error";
        try
        {
            await next(context);
            statusCode = (int)HttpStatusCode.InternalServerError;

            // Check if response is too many requests (429)
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                statusCode = (int)HttpStatusCode.TooManyRequests;
                message = "Too many requests. Please try again later.";
                title = "Too Many Requests";                
            }
            else if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                statusCode = (int)HttpStatusCode.NotFound;
                message = "The requested resource was not found.";
                title = "Not Found";
            }
            else if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "The request could not be understood or was missing required parameters.";
                title = "Bad Request";
            }
            else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "Authentication failed or user does not have permissions for the desired action.";
                title = "Unauthorized";
            }
            else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                statusCode = (int)HttpStatusCode.Forbidden;
                message = "Authentication succeeded but authenticated user does not have access to the requested resource.";
                title = "Forbidden";
            }

            await ModifyHeader(context, title, statusCode, message);

        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            //A default error message
            await ModifyHeader(context, title, statusCode, message);
        }
    }

    private static async Task ModifyHeader(HttpContext context, string title, int statusCode, string message)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Title = title,
                Message = message
            }), CancellationToken.None);
        }        
        return;
    }
}
