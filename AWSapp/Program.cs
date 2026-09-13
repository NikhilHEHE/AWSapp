using AWSapp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Controllers
// ----------------------------------------------------

builder.Services.AddControllers();


// ----------------------------------------------------
// Database
// ----------------------------------------------------

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string 'DefaultConnection' was not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
        });
});


// ----------------------------------------------------
// Swagger
// ----------------------------------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ----------------------------------------------------
// Health Checks
// ----------------------------------------------------

builder.Services
    .AddHealthChecks().AddDbContextCheck<AppDbContext>();


var app = builder.Build();


// ----------------------------------------------------
// Swagger
// ----------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ----------------------------------------------------
// Health Check
// ----------------------------------------------------

// This endpoint will later be used by the
// AWS Application Load Balancer.

app.MapHealthChecks("/health");


// ----------------------------------------------------
// Controllers
// ----------------------------------------------------

app.MapControllers();


// ----------------------------------------------------
// Start Application
// ----------------------------------------------------

app.Run();