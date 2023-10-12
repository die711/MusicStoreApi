using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
using MusicStore.Repositories.Implementations;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Implementations;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MusicStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MusicStoreDb"));
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Mapear el contenido del archivo appsettings .json en una clase
builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<GenreProfile>();
    config.AddProfile<ConcertProfile>();
});

builder.Services.AddTransient<IGenreRepository, GenreRepository>();
builder.Services.AddTransient<IConcertRepository, ConcertRepository>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();

builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IConcertService, ConcertService>();
builder.Services.AddTransient<IUserService, UserService>();

//builder.Services.AddTransient<IFileUploader, AzureBlobStorageUploader>();

if (builder.Environment.IsDevelopment())
    builder.Services.AddTransient<IFileUploader, FileUploader>();
else
    builder.Services.AddTransient<IFileUploader, AzureBlobStorageUploader>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();