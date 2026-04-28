using Microsoft.EntityFrameworkCore;
using rest.Data;
using rest.Middleware;
using rest.Repositories;

var builder = WebApplication.CreateBuilder(args);

//CORS config
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<IDirectorsRepository, DirectorsRepository>();
builder.Services.AddScoped<IProductionCompaniesRepository, ProductionCompaniesRepository>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Specify headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "default-src 'none'; frame-ancestors 'none'; base-uri 'none';"
    );
    await next();
});
//XXS prevention
app.UseMiddleware<XssMiddleware>();

//cors
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
