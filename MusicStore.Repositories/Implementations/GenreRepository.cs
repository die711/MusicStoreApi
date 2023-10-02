using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using MusicStore.DataAccess;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;

namespace MusicStore.Repositories.Implementations;

public class GenreRepository : RepositoryBase<Genre>, IGenreRepository
{
    public GenreRepository(MusicStoreDbContext context) : base(context)
    {
    }
    
    // public async Task<long> AddAsync(Genre genre)
    // {
    //     await Context.Set<Genre>().AddAsync(genre);
    //     await Context.SaveChangesAsync();
    //     
    //     
    //     //tarea en background
    //     var tareaAdicional = Task.Factory.StartNew(async() =>
    //     {
    //         await Task.Delay(2000);
    //         System.Diagnostics.Debug.WriteLine("otro proceso en otro hilo");
    //     });
    //
    //     await tareaAdicional;
    //
    //     return genre.Id;
    // }
    
}