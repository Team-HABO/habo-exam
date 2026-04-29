using Microsoft.EntityFrameworkCore;
using soap.Data;
using soap.Services;
using SoapCore;



var builder = WebApplication.CreateBuilder(args);

// Register PostgreSQL + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new Exception("CONNECTION_STRING not defined")));

// Register SoapCore and the library service implementation
builder.Services.AddSoapCore();
builder.Services.AddScoped<IArtistService, ArtistService>();

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

// Register the SOAP endpoint
app.UseSoapEndpoint<IArtistService>("/ArtistService.asmx", new SoapEncoderOptions());

// Health check endpoint
app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
