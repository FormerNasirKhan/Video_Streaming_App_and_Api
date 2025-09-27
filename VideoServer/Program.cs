using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using VideoApi.Services;
using VideoApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Video API",
        Version = "v1",
        Description = "Simple API for listing and streaming videos from local folder"
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// EF Core
builder.Services.AddDbContext<VideoStreamDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("VideoStreamDb");
    opt.UseSqlServer(cs);
});

// Auth service
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Video API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
