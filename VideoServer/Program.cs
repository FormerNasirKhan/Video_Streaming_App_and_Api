using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// ? Add Swagger/OpenAPI support
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

// ? Add CORS (needed for WPF app to call API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ? Enable Swagger UI always (Dev + Prod)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Video API v1");
    options.RoutePrefix = "swagger"; // so URL = /swagger
});

// ? Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
