using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
builder.Services.AddDbContext<BloggieDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
    x=>x.MigrationsHistoryTable("_EfMigrations", Configuration.GetSection("Schema").GetSection("BloggieSchema").Value)));
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
