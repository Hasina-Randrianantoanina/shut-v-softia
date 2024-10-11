using SHUT.Api.Endpoints;
using SHUT.Core.Application;
using SHUT.Core.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DBSettings>(builder.Configuration.GetSection("DbSettings"));
builder.Services.Configure<LDapSettings>(builder.Configuration.GetSection("LDapSettings"));
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
builder.Services.Configure<InfluxSettings>(builder.Configuration.GetSection("InfluxSettings"));
// Add services to the container.
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<LDapIdentifier>();
builder.Services.AddSingleton<InfluxService>();
//builder.Services.AddHostedService<ConnexionEnregistreur>();
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<VisualisationService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Error");
    //    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseCors();

app.MapGet(
    "/HealthCheck",
    () =>
    {
        return "Hello from Api";
    }
);

app.MapGroup("Authenticate/").MapAuthentication()
.WithTags("Authnetication EndPoints")
.WithOpenApi();

app.MapGroup("Users/").MapUsersApi()
.WithTags("Users EndPoints")
.WithOpenApi();

app.MapGroup("data/").MapInfluxDataApi()
.WithTags("Users EndPoints")
.WithOpenApi();




app.Run();


