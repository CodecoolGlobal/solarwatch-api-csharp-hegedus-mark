using Microsoft.EntityFrameworkCore;
using SolarWatch.Configuration;
using SolarWatch.Data;
using SolarWatch.Services;
using dotenv.net;
using Serilog;
using SolarWatch.Data.Repositories;


DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders(); // Optionally clear default providers
builder.Logging.AddConsole(); // Add console logging
builder.Logging.AddDebug(); // Add debug logging

// Set up Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SolarWatchDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DbConnection");
    options.UseSqlServer(connectionString);
});
builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ExternalApiSettings"));
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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}