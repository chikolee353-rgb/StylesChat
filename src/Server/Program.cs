using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Server.Hubs;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Simple symmetric key for demo purposes - in production use secure secret store
var jwtKey = configuration["Jwt:Key"] ?? "super_secret_dev_key_change_me";
var jwtIssuer = configuration["Jwt:Issuer"] ?? "masvegas.chat";

// Services
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Configure EF Core with SQLite for local/dev persistence
var connString = configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=MasVegasChatDb;Trusted_Connection=True;MultipleActiveResultSets=true";
// Configure EF Core to use SQL Server. For Azure, replace connection string with managed SQL DB.
builder.Services.AddDbContext<Server.Data.AppDbContext>(options =>
    options.UseSqlServer(connString));

// Register EF-backed services
builder.Services.AddScoped<Server.Services.EfUserService>();
builder.Services.AddScoped<Server.Services.EfMessageService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true,
    };

    // Allow SignalR connections to send access token via query string
    options.Events = new JwtBearerEvents
    {
    OnMessageReceived = context =>
    {
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;
        // Accept access token for requests targeting the SignalR hub endpoint
        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
        {
            context.Token = accessToken;
        }
        return Task.CompletedTask;
    }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure database is created (use migrations in real projects)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Server.Data.AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Map SignalR hub at /chatHub for client connections
app.MapHub<ChatHub>("/chatHub");

app.Run();
