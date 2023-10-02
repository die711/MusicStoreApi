using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class GenreService : IGenreService
{
    public Task<BaseResponseGeneric<ICollection<GenreDtoResponse>>> ListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(long id)
    {
        throw new NotImplementedException();
    }
}