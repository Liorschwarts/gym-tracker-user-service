using Microsoft.EntityFrameworkCore;
using GymTracker.UserService.Data;
using GymTracker.UserService.Services;  // ← הוסף את זה

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// הוספת Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// הוספת DbContext עם PostgreSQL
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

 builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        // מפעיל Swagger
    app.UseSwaggerUI();      // מפעיל הUI של Swagger
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();