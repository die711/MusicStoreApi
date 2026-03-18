using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
using MusicStore.Repositories.Implementations;

namespace MusicStore.UnitTests;

public class UnitOfWorkTests
{
    private async Task<MusicStoreDbContext> ArrangeDatabase()
    {
        var options = new DbContextOptionsBuilder<MusicStoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new MusicStoreDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        return dbContext;
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldCommitToDatabase()
    {
        // Arrange
        var dbContext = await ArrangeDatabase();
        var unitOfWork = new UnitOfWork(dbContext);
        var repository = new GenreRepository(dbContext);

        var genre = new Genre
        {
            Name = "Unit Test Genre",
            Status = true
        };

        // Act
        // AddAsync does not call SaveChangesAsync anymore, so count should remain same 
        // until we call unitOfWork.SaveChangesAsync()
        await repository.AddAsync(genre);

        // Assert before save
        var countBefore = await dbContext.Set<Genre>().CountAsync(x => x.Name == "Unit Test Genre");
        Assert.Equal(0, countBefore); // Entity is tracked but not in the physical db yet

        // Act - Call UnitOfWork
        var result = await unitOfWork.SaveChangesAsync();

        // Assert after save
        var countAfter = await dbContext.Set<Genre>().CountAsync(x => x.Name == "Unit Test Genre");
        Assert.True(result > 0);
        Assert.Equal(1, countAfter);
    }
}
