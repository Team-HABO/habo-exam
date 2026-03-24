using Microsoft.EntityFrameworkCore;
using soap.Data;
using soap.Services;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Register SQLite + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=data/library.db"));

// Register SoapCore and the library service implementation
builder.Services.AddSoapCore();
builder.Services.AddScoped<ILibraryService, LibraryService>();

var app = builder.Build();

// Register the SOAP endpoint
app.UseSoapEndpoint<ILibraryService>("/LibraryService.asmx", new SoapEncoderOptions());

// Health check endpoint
app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
