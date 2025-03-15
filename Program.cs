using BulkInsertAPI.Infrastructure.DBContext;
using BulkInsertAPI.Infrastructure.Repos;
using BulkInsertAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore; // Add this using directive

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<WorkerRepository>();
builder.Services.AddScoped<ZoneRepository>();
builder.Services.AddScoped<WorkerZoneAssignmentRepository>();
builder.Services.AddScoped<AssigmentService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
