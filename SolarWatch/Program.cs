using System.Text;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Configuration;
using SolarWatch.Data;
using SolarWatch.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SolarWatch.Controllers;
using SolarWatch.Data.Repositories;
using SolarWatch.Services.Authentication;


DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Configure services and other settings
ConfigureSettings();
ConfigureLogging();
AddServices();
ConfigureSwagger();
AddDbContexts();
AddAuthentication();
AddIdentity();

if (builder.Environment.IsDevelopment())
{
    AddCors();
}

var app = builder.Build();

// Configure the HTTP request pipeline
AddRoles();
ConfigureApp();

app.Run();

Log.CloseAndFlush();

return;


void ConfigureSettings()
{
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ExternalApiSettings"));
    builder.Services.Configure<RoleSettings>(builder.Configuration.GetSection("RoleSettings"));
}

// Configuration Methods
void ConfigureLogging()
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    // Set up Serilog
    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    builder.Host.UseSerilog();
}

void AddServices()
{
    builder.Services.AddControllers();

    builder.Services.AddSingleton<HttpClient>();
    builder.Services.AddSingleton(new ApiServiceConfiguration
    {
        MaxRetries = 3,
        RetryDelayMilliseconds = 1000
    });
    builder.Services.AddTransient<IApiService, ApiService>();
    builder.Services.AddTransient<IGeocodeApiService, GeocodeApiService>();
    builder.Services.AddTransient<ISunriseSunsetApiService, SunriseSunsetApiService>();
    builder.Services.AddTransient<ICityRepository, CityRepository>();
    builder.Services.AddTransient<ICityDataService, CityDataService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<AuthenticationSeeder>();
}

void ConfigureSwagger()
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "SolarWatch API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer",
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        });
    });
}

void AddDbContexts()
{
    builder.Services.AddDbContext<SolarWatchDbContext>(options =>
    {
        options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings__DbConnection"));
    });

    builder.Services.AddDbContext<UsersContext>(options =>
    {
        options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings__DbConnection"));
    });
}

void AddAuthentication()
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] ??
                                           throw new InvalidOperationException())
                ),
            };
        });
}

void AddIdentity()
{
    builder.Services.AddIdentityCore<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<UsersContext>();
}

void AddCors()
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin"
            , policyBuilder =>
            {
                policyBuilder.WithOrigins(configuration["AllowedOrigin"] ?? throw new InvalidOperationException())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
    });
}

void ConfigureApp()
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
        app.UseCors("AllowSpecificOrigin");
        Log.Information("Running ASP.NET Core Web API in Development mode");
        Log.Information(configuration["JwtSettings:SecretKey"]);
        
        
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        var roleSettings = services.GetRequiredService<IOptions<RoleSettings>>().Value;

        Log.Information($"AdminRole: {roleSettings.AdminRoleName}");
        Log.Information($"UserRole: {roleSettings.UserRoleName}");
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}


void AddRoles()
{
    using var scope = app.Services.CreateScope();

    var authenticationSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();
    authenticationSeeder.AddRoles();
    authenticationSeeder.AddAdmin();
}

public partial class Program { }