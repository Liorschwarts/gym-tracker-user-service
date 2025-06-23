using Microsoft.EntityFrameworkCore;
using GymTracker.UserService.Data;
using GymTracker.UserService.Services;
using GymTracker.UserService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// If no connection string, use environment-specific defaults
if (string.IsNullOrEmpty(connectionString))
{
    if (builder.Environment.IsDevelopment())
    {
        connectionString = "Host=localhost;Database=GymTrackerDB;Username=postgres;Password=dev123";
        Console.WriteLine("🔧 Using default development database connection");
    }
    else
    {
        throw new InvalidOperationException("❌ Database connection string is required for production");
    }
}

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

// JWT Configuration with fallbacks
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];
if (string.IsNullOrEmpty(jwtSecretKey))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtSecretKey = "Development-JWT-Secret-Key-For-Local-Team-32-Characters-Long-Only";
        Console.WriteLine("🔧 Using default development JWT secret");
    }
    else
    {
        throw new InvalidOperationException("❌ JWT Secret Key is required for production");
    }
}

// Register services
builder.Services.AddScoped<IUserService, GymTracker.UserService.Services.UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

    try
    {
        Console.WriteLine("🔄 Checking database...");
        context.Database.EnsureCreated();
        Console.WriteLine("✅ Database ready!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database error: {ex.Message}");
        Console.WriteLine("💡 Make sure PostgreSQL is running (try: docker-compose up -d)");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service V1");
        c.RoutePrefix = string.Empty; // Swagger at root URL
    });
}

app.UseHttpsRedirection();
app.MapControllers();

Console.WriteLine($"🚀 GymTracker UserService running in {builder.Environment.EnvironmentName} mode");
Console.WriteLine($"📍 Database: {connectionString.Split(';')[0]}");

app.Run();