using interviewProject.Data;
using interviewProject.Events;
using interviewProject.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configure services
// Add DbContext service with SQL Server configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register custom services
// Add CountryService as a scoped dependency
builder.Services.AddScoped<ICountryService, CountryService>();

// Add essential services for controllers, HTTP clients, and background services
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<MbaOptionsService>();

// Register API documentation services (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register event receivers found in the assembly
EventManager.RegisterEventReceivers(builder.Services);

var app = builder.Build();

// Apply database migrations and initialize event receivers at startup
using (var scope = app.Services.CreateScope())
{
    // Apply pending migrations to the database
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    // Initialize event receivers to start listening to events
    EventManager.InitializeEventReceivers(scope.ServiceProvider);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable middleware for serving generated Swagger as a JSON endpoint
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authorization middleware
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Run the application
app.Run();

public partial class Program { }