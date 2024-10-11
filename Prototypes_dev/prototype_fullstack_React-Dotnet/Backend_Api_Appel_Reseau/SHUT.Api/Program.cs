using SHUT.Api.Endpoints;
using SHUT.Core.Application;
using SHUT.Core.Application.Interfaces;
using SHUT.Core.Data;
using ReseauDEAOperations;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Configuration des services
builder.Services.Configure<DBSettings>(builder.Configuration.GetSection("DbSettings"));
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<VisualisationService>();

// Configuration de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
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


app.MapGet("/HealthCheck", () => "Hello from Api");

app.MapGroup("Visualisation/")
   .MapVisualisationApi()
   .WithTags("Visu EndPoints")
   .WithOpenApi()
   .RequireCors("AllowAll");

app.Run();