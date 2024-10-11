using SHUT.Api.Endpoints;
using SHUT.Core.Application.Interfaces;
using SHUT.Core.Data;
using ReseauDEAOperations;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using SHUT.Core.Application;
using SHUT.Core.Application.Reseau;
using SHUT.Core.Application.Historique;
using SHUT.Core.Application.Administration;
using SHUT.Core.Application.Defaut;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration des services

builder.Services.Configure<ConnectionStringOptions>(builder.Configuration.GetSection("ConnectionStrings"));
// builder.Services.Configure<DBSettings>(builder.Configuration.GetSection("DbSettings")); // a tester commenté en utilisant exclusivement connectionStrings
//builder.Services.Configure<ShutDbSettings>(builder.Configuration.GetSection("ShutDbSettings"));
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
builder.Services.Configure<LDapSettings>(builder.Configuration.GetSection("LDapSettings"));

builder.Services.AddSingleton<DataBaseSettingsProvider>();
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<ArchiveDbContext>();
builder.Services.AddSingleton<LDapIdentifier>();
//builder.Services.AddSingleton<ShutDbContext>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddSingleton<VisualisationService>();
builder.Services.AddScoped<StationService>();
builder.Services.AddScoped<EnregistreurService>();
builder.Services.AddScoped<VoiesService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DefautsActifsService>();
builder.Services.AddScoped<DefautService>();

builder.Services.AddScoped<AlerteService>();
builder.Services.AddScoped<PerteService>();
builder.Services.AddScoped<TraitementService>();
builder.Services.AddScoped<PreselectionService>();

builder.Services.AddSingleton<IOrchestrateur, OrchestrateurAppels>();
builder.Services.AddScoped<OrchestationReseauService>();



// builder.Services.AddScoped<EnumService>();


// Configuration de l'authentification JWT
var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ALL", policy =>
        policy.RequireRole("OPERATEUR", "ADMIN", "CONSULTATION", "VALIDEUR"));
});


// Configuration de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var result = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Une erreur interne s'est produite.",
            DetailedMessage = exception?.Message
        };

        await context.Response.WriteAsJsonAsync(result);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("Visualisation/")
   .MapVisualisationApi()
   .WithTags("Visu EndPoints")
   .WithOpenApi()
   .RequireCors("AllowAll");

app.MapGroup("Stations/")
   .MapStationsApi()
   .WithTags("Stations")
   .WithOpenApi()
   .RequireCors("AllowAll");

app.MapGroup("Enregistreurs/")
   .MapEnregistreursApi()
   .WithTags("Enregistreurs")
   .WithOpenApi()
   .RequireCors("AllowAll");

app.MapGroup("Authentication/")
    .MapAuthentication()
    .WithTags("Authentication")
    .WithOpenApi()
    .RequireCors("AllowAll");

app.MapGroup("Users/")
   .MapUsersApi()
   .WithTags("Users")
   .WithOpenApi()
   .RequireCors("AllowAll");

app.MapGroup("Voies/")
    .MapVoiesApi()
    .WithTags("Voies")
    .WithOpenApi()
    .RequireCors("AllowAll");

app.MapGroup("Defauts/")
    .MapDefautsActifsApi()
    .WithTags("Defauts")
    .WithOpenApi()
    .RequireCors("AllowAll");

app.MapGroup("Battements/")
    .MapDefautApi()
    .WithTags("Battements")
    .WithOpenApi()
    .RequireCors("AllowAll");

app.MapGroup("Alertes/")
    .MapAlerteApi()
    .WithTags("Alertes")
    .WithOpenApi()
    .RequireCors("AllowAll");

app.MapGroup("Pertes/")
    .MapPerteApi()
    .WithTags("Perte")
    .WithOpenApi()
    .RequireCors("AllowAll");

app.MapGroup("Traitements/")
    .MapTraitementApi()
    .WithTags("Traitements")
    .WithOpenApi()
    .RequireCors("AllowAll");

app.MapGroup("/preselections")
   .MapPreselectionApi()
   .WithTags("Preselections")
   .WithOpenApi()
   .RequireCors("AllowAll");


// app.MapGroup("Enums/")
//     .MapVoiesApi()
//     .WithTags("Enums")
//     .WithOpenApi()
//     .RequireCors("AllowAll");

app.Run();