using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class GoogleAuthMiddleware
{
    private readonly RequestDelegate _next;

    public GoogleAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, GoogleHandler googleHandler)
    {
        System.Console.WriteLine("GoogleAuthMiddleware is called");
        await _next(context);
    }
}
