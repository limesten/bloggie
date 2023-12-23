using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
builder.Services.AddDbContext<BloggieDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ApiKeyAuthMiddleware>();

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/healthz");
app.MapPost("/users", async (BloggieDbContext context, User user) => 
{
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

app.Run();
