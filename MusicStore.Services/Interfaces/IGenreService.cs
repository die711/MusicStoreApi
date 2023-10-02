using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface IGenreService
{

    Task<BaseResponseGeneric<ICollection<GenreDtoResponse>>> ListAsync();
    Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(long id);
}