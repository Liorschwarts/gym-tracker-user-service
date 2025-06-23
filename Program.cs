using GymTracker.UserService.Data;
using GymTracker.UserService.Interfaces;
using GymTracker.UserService.Middleware;
using GymTracker.UserService.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/userservice-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<UserDbContext>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "GymTracker User Service",
        Version = "v1",
        Description = "User management and authentication API"
    });
});

// Database Configuration with fallbacks
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
                    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    if (builder.Environment.IsDevelopment())
    {
        connectionString = "Host=localhost;Database=GymTrackerDB;Username=postgres;Password=dev123";
        Log.Warning("Using fallback development database connection - consider using environment variables");
    }
    else
    {
        throw new InvalidOperationException("DATABASE_CONNECTION_STRING environment variable is required for production");
    }
}

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

// JWT Configuration with fallbacks
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? builder.Configuration["Jwt:SecretKey"];

if (string.IsNullOrEmpty(jwtSecretKey))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtSecretKey = "Development-JWT-Secret-Key-For-Local-Team-32-Characters-Long-Only";
        Log.Warning("Using fallback development JWT secret - consider using environment variables");
    }
    else
    {
        throw new InvalidOperationException("JWT_SECRET_KEY environment variable is required for production");
    }
}
// Register services
builder.Services.AddScoped<IUserService, GymTracker.UserService.Services.UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Rate limit עבור login attempts
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.PermitLimit = 5; // 5 נסיונות
        opt.Window = TimeSpan.FromMinutes(1); // לדקה
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // אין queue - block מיד
    });

    // Global rate limit
    options.AddFixedWindowLimiter("GlobalPolicy", opt =>
    {
        opt.PermitLimit = 100; // 100 requests
        opt.Window = TimeSpan.FromMinutes(1); // לדקה
    });
});

var app = builder.Build();

// Global Exception Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

    try
    {
        Log.Information("Checking database...");
        context.Database.EnsureCreated();
        Log.Information("Database ready!");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Database error: {Message}", ex.Message);
        Log.Information("Make sure PostgreSQL is running (try: docker-compose up -d)");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();

// Add Health Checks endpoint
app.MapHealthChecks("/health");

app.MapControllers();

try
{
    var dbHost = connectionString.Split(';').FirstOrDefault(x => x.StartsWith("Host="))?.Replace("Host=", "") ?? "Unknown";
    Log.Information("GymTracker UserService running in {Environment} mode", builder.Environment.EnvironmentName);
    Log.Information("Database Host: {DbHost}", dbHost);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}