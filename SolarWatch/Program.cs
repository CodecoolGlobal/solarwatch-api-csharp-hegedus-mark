using SolarWatch.Configuration;
using SolarWatch.DTOs;
using SolarWatch.Models;
using SolarWatch.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.Configure<ExternalApiSettings>(builder.Configuration.GetSection("ExternalApiSettings"));
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton(new ApiServiceConfiguration
{
    MaxRetries = 3,
    RetryDelayMilliseconds = 1000
});
builder.Services.AddTransient<IApiService<SunriseSunsetApiResponseDto>, ApiService<SunriseSunsetApiResponseDto>>();
builder.Services.AddTransient<IApiService<Coordinates>, ApiService<Coordinates>>();
builder.Services.AddTransient<IGeocodeApiService, GeocodeApiService>();
builder.Services.AddTransient<ISunriseSunsetApiService, SunriseSunsetApiService>();


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