using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using rest.Data;
using rest.Repositories;
using rest.Services;
using StackExchange.Redis;

// Checks for an environment variable first, then defaults to a local path
var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");

// If not in the current directory, try walking up (common for local dev)
if (!File.Exists(dotenv))
{
    dotenv = Path.Combine(root, "..", "..", "..", ".env");
}

if (File.Exists(dotenv))
{
    DotNetEnv.Env.Load(dotenv);
}

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
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDirectorsRepository, DirectorsRepository>();
builder.Services.AddScoped<IProductionCompaniesRepository, ProductionCompaniesRepository>();
builder.Services.AddControllers();
// XSS sanitation
builder.Services.AddScoped<IHtmlSanitizer, HtmlSanitizer>();
//builder.Services.AddSingleton<IConnectionMultiplexer>(
//    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!));
//var redisConn = builder.Configuration["Redis:ConnectionString"]!;
var redisConn = builder.Configuration["REDIS_CONNECTION_STRING"]! ?? throw new InvalidOperationException("Redis connection string is null"); 
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConn));


builder.Services.AddHttpClient();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "https://habo-integration-exam.us.auth0.com/";

    options.Audience = "https://habo-fake-frontend.sk";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        // Ensure the token has not been tampered with
        ValidateIssuerSigningKey = true
    };
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//This registers the Authorization Services into the Dependency Injection (DI) container.
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Add headers to request
app.Use(async (context, next) =>
{
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "default-src 'none'; frame-ancestors 'none'; base-uri 'none';"
    );
    await next();

});

//cors
app.UseCors("AllowFrontend");

// so it checks if endpoint has [Authorize]
app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<TokenBlacklistMiddleware>();

app.MapControllers();

app.Run();
