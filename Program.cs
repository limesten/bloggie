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

app.Run();
