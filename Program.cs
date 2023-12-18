using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
builder.Services.AddDbContext<BloggieDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/healthz");
app.MapPost("/users", async (BloggieDbContext context, User user) => 
{
    context.Users.Add(user);
    await context.SaveChangesAsync();
    return Results.Ok(await context.Users.ToListAsync());
});
app.MapGet("/users", async (HttpContext httpContext, BloggieDbContext context) =>
{
    var apiKey = httpContext.Request.Headers["x-api-key"].ToString();
    if (apiKey == "")
    {
        return Results.BadRequest("Api key missing");
    }
    var user = await context.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);

    if (user != null) 
    {
        return Results.Ok(user);
    }
    return Results.NotFound("User not found");
});

app.Run();
