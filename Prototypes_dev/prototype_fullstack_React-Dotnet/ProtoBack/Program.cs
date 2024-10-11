using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBack.Data;
using ProtoBack.Repositories;
using ProtoBack.Api;
using InfluxDB.Client;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowSpecificOrigin",
        builder =>
            builder
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
    );
});

// Configuration EF
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("dbConnect"))
);

// DapperContext & UserRepository
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<UserRepository>();

// Add TimescaleDb
builder.Services.AddDbContext<TimescaleContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("TimescaleDb"));
});

// Ajout du client InfluxDB
const string influxUrl = "http://localhost:8086";
const string token = "b6XPK2X8-H5oagLI_pZenDO1aionWRSvx6zYI0Z1Yd_YDYGSOtV-JpULFILZ8GHep7bATgNzKViUX7sbsrrEPQ==";
const string bucket = "TestShut";
const string org = "S";

var clientOptions = new InfluxDBClientOptions.Builder()
    .Url(influxUrl)
    .AuthenticateToken(token.ToCharArray())
    .Org(org)
    .Bucket(bucket)
    .Build();
var influxDBClient = InfluxDBClientFactory.Create(clientOptions);

builder.Services.AddSingleton(influxDBClient);

// Add authorization services
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();

// Enregistrement des API minimalistes
app.MapInfluxDBApi();
app.MapUserApi();
app.MapTimescaleApi();

app.Run();

app.Lifetime.ApplicationStopping.Register(() =>
{
    influxDBClient.Dispose();
});
