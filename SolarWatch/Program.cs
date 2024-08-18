using System.Text;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Configuration;
using SolarWatch.Data;
using SolarWatch.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SolarWatch.Data.Repositories;
using SolarWatch.Services.Authentication;


DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure services and other settings
ConfigureLogging();
AddServices();
ConfigureSwagger();
AddDbContexts();
AddAuthentication();
AddIdentity();

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigureApp();

app.Run();


// Configuration Methods
void ConfigureLogging()
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    // Set up Serilog
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
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
                ValidIssuer = "apiWithAuthBackend",
                ValidAudience = "apiWithAuthBackend",
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ??
                                           throw new InvalidOperationException("JWT_SECRET_KEY not found!"))
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
        .AddEntityFrameworkStores<UsersContext>();
}

void ConfigureApp()
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}

public partial class Program
{
}