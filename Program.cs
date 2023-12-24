using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
builder.Services.AddDbContext<BloggieDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
var app = builder.Build();

app.UseWhen(
    context => !(context.Request.Path.StartsWithSegments("/feeds") && context.Request.Method == "GET"),
    builder => builder.UseMiddleware<ApiKeyAuthMiddleware>()
);

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/healthz");
app.MapPost("/users", async (BloggieDbContext context, User user) =>
{
    user.ApiKey = new ApiKeyService().GenerateApiKey();
    context.Users.Add(user);
    await context.SaveChangesAsync();
    return Results.Ok(await context.Users.ToListAsync());
});
app.MapGet("/users", async (HttpContext httpContext) =>
{
    if (httpContext.Items.TryGetValue("User", out var user))
    {
        await httpContext.Response.WriteAsJsonAsync(user);
    }
    else
    {
        httpContext.Response.StatusCode = 404;
        await httpContext.Response.WriteAsync("User not found");
    }
});
app.MapPost("/feeds", async (HttpContext httpContext, BloggieDbContext context, FeedRequest feedRequest) =>
{
    if (httpContext.Items.TryGetValue("User", out var userObject) && userObject is User user)
    {
        var feed = new Feed
        {
            Name = feedRequest.Name,
            Url = feedRequest.Url,
            UserId = user.Id,
        };

        context.Feeds.Add(feed);
        await context.SaveChangesAsync();
        return Results.Ok(await context.Feeds.ToListAsync());
    }
    else
    {
        return Results.NotFound("User not found");
    }
});
app.MapGet("/feeds", async (HttpContext httpContext, BloggieDbContext context) =>
{
    var feeds = await context.Feeds.ToListAsync();
    return Results.Ok(feeds);
});

app.Run();
