using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MusicStore.DataAccess;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Implementations;
using MusicStore.Services.Implementations;
using MusicStore.Services.Profiles;

namespace MusicStore.UnitTests;

public class MusicStoreTests
{
    [Fact]
    public void SumaTest()
    {
        //AAA
        //Arrange
        int a = 6;
        int b = 7;

        //Act
        var suma = a + b;
        var expected = 13;

        //Assert
        Assert.Equal(expected, suma);
    }

    private static async Task<MusicStoreDbContext> ArrangeDatabase()
    {
        var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new MusicStoreDbContext(options);

        await dbContext.Database.EnsureCreatedAsync(); //Ejecuta migraciones
        return dbContext;
    }

    [Fact]
    public async Task GenreRepositoryListTest()
    {
        var dbContext = await ArrangeDatabase();
        var genreRepository = new GenreRepository(dbContext);
        var response = await genreRepository.ListAsync(p => p.Status);

        Assert.Equal(5, response.Count);
    }

    [Fact]
    public async Task GenreRepositoryFirstTest()
    {
        var dbContext = await ArrangeDatabase();
        var genreRepository = new GenreRepository(dbContext);
        var response = await genreRepository.ListAsync(p => p.Status);

        var genre = response.FirstOrDefault();

        Assert.NotNull(genre);
    }

    [Fact]
    public async Task ListGenreTest()
    {
        var dbContext = await ArrangeDatabase();
        var genreRepository = new GenreRepository(dbContext);

        var mockLogger = new Mock<ILogger<GenreService>>();
        var mapper = new Mapper(new MapperConfiguration(p => p.AddProfile(typeof(GenreProfile))));

        var genreService = new GenreService(genreRepository, mockLogger.Object, mapper);

        var response = await genreService.ListAsync();

        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);
        Assert.NotEmpty(response.Data);
        Assert.Equal(5, response.Data.Count);
    }

    [Fact]
    public async Task CheckIfGenreIsDeleteAfterMigrationTest()
    {
        var dbContext = await ArrangeDatabase();

        var genre = await dbContext.Set<Genre>().FirstOrDefaultAsync(p => p.Name == "Rock");

        if (genre != null)
        {
            dbContext.Set<Genre>().Remove(genre);
            await dbContext.SaveChangesAsync();
        }

        var count = await dbContext.Set<Genre>().CountAsync();
        var expected = 4;
        
        Assert.Equal(expected, count);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(10, false)]
    public async Task GenreGetByIdTest(long id, bool expected)
    {
        var dbContext = await ArrangeDatabase();
        var repository = new GenreRepository(dbContext);
        var mockLogger = new Mock<ILogger<GenreService>>();

        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<GenreDtoResponse>(It.IsAny<Genre>())).Returns(new GenreDtoResponse());
        var service = new GenreService(repository, mockLogger.Object, mapper.Object);
        var response = await service.FindByIdAsync(id);
        
        Assert.Equal(expected, response.Success);
    }
}

