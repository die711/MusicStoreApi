using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(config =>
{
config.AddProfile<GenreProfile>();
});

builder.Services.AddTransient<IGenreRepository, GenreRepository>();

builder.Services.AddTransient<IGenreService, GenreService>();


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