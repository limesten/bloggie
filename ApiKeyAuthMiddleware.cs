using Microsoft.EntityFrameworkCore;

public class ApiKeyAuthMiddleware 
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, BloggieDbContext context)
    {
        var apiKey = httpContext.Request.Headers["x-api-key"].ToString();
        if (apiKey == "")
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Api key missing");
            return;
        }
        var user = await context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);

        if (user == null) 
        {
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsync("Invalid API key");
            return;
        }

        httpContext.Items["User"] = user;
        await _next(httpContext);
    }
}
