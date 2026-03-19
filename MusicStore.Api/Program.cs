using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using FluentValidation;
using FluentValidation.AspNetCore;
using MusicStore.Api.Endpoints;
using MusicStore.Dto.Validations;
using MusicStore.DataAccess;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Services.Implementations;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Profiles;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MusicStoreDb");
var corsConfig = "MusicStoreAPI";

var logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Information)
    .WriteTo.File("..\\log.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] - {Message} {NewLine} {Exception}",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Warning)
    .WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("MusicStoreDb"),
        new MSSqlServerSinkOptions()
        {
            AutoCreateSqlTable = true,
            TableName = "ApiLogs"
        }, restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();

builder.Logging.AddSerilog(logger);


builder.Services.AddCors(setup =>
{
    setup.AddPolicy(corsConfig, policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
    });


});



// Add services to the container.

builder.Services.AddDbContext<MusicStoreDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

//Aca se especifica la clase de usuario personalizado

builder.Services.AddIdentity<MusicStoreUserIdentity, IdentityRole>(policies =>
    {
        policies.Password.RequireDigit = false;
        policies.Password.RequireLowercase = false;
        policies.Password.RequireUppercase = false;
        policies.Password.RequireNonAlphanumeric = false;
        policies.Password.RequiredLength = 5;
        policies.User.RequireUniqueEmail = true;

        //politicas de bloqueo
        policies.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        policies.Lockout.MaxFailedAccessAttempts = 3;
        policies.Lockout.AllowedForNewUsers = true;
    }).AddEntityFrameworkStores<MusicStoreDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<GenreDtoRequestValidator>();
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Music Store API",
            Version = "v1"
        };

        document.Components ??= new OpenApiComponents();
        document.Components!.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Description = "Autenticacion por JWT usando como ejemplo en el Header: Authorizacion: Bearer {token}",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

//Mapear el contenido del archivo appsettings .json en una clase
builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<GenreProfile>();
    config.AddProfile<ConcertProfile>();
    config.AddProfile<SaleProfile>();
});




builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IConcertService, ConcertService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ISaleService, SaleService>();
builder.Services.AddTransient<IEmailService, EmailService>();
//builder.Services.AddTransient<IFileUploader, AzureBlobStorageUploader>();

if (builder.Environment.IsDevelopment())
    builder.Services.AddTransient<IFileUploader, FileUploader>();
else
    builder.Services.AddTransient<IFileUploader, AzureBlobStorageUploader>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = "Bearer";
    x.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(x =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException());
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<MusicStore.Api.Middlewares.ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(corsConfig);

app.MapControllers();

app.MapHomeEndPoints();
app.MapReports();


//Sales

app.MapPost("Sales",
    async (ISaleService service, HttpContext context, ILogger<Program> logger, SaleDtoRequest request) =>
    {
        var email = context.User.Identity?.Name ?? "unknown";

        logger.LogInformation("Autenticado como {Name}", email);
        logger.LogInformation("El token vencera el dia {Date}",
            context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Expiration)?.Value ?? "N/A");

        var response = await service.AddAsync(email, request);
        return response.Success ? Results.Ok(response) : Results.BadRequest(response);
        // return Results.Ok();
    }).RequireAuthorization();


//Users
app.MapPost("Users/Register", async (IUserService service, RegisterDtoRequest request) =>
{
    var response = await service.RegisterAsync(request);
    return response.Success ? Results.Ok(response) : Results.BadRequest(response);
}).AddEndpointFilter<MusicStore.Api.Filters.ValidationFilter<RegisterDtoRequest>>();

app.MapPost("Users/Login", async (IUserService service, LoginDtoRequest request) =>
{
    var response = await service.LoginAsync(request);
    return response.Success ? Results.Ok(response) : Results.Json(response, statusCode: 401);
});

app.MapPost("Users/SendTokenToResetPassword", async (IUserService service, DtoRequestPassword request) =>
{
    var response = await service.RequestTokenToResetPasswordAsync(request);
    return response.Success ? Results.Ok(response) : Results.BadRequest(response);
});

app.MapPost("Users/ResetPassword", async (IUserService service, DtoResetPassword request) =>
{
    var response = await service.ResetPasswordAsync(request);
    return response.Success ? Results.Ok(response) : Results.BadRequest(response);
});

app.MapPost("Users/ChangePassword", async (IUserService service, DtoChangePassword request) =>
{
    var response = await service.ChangePasswordAsync(request);
    return response.Success ? Results.Ok(response) : Results.BadRequest(response);
});

if (builder.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    {
        logger.Information("Configurando las migraciones");
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<MusicStoreDbContext>();
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.Error("Error al conectarse a {ConnectionString} {Message}", connectionString, ex.Message);
        }
    }
}

app.Run();