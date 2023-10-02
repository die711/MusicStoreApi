using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface IGenreService
{
    Task<BaseResponseGeneric<ICollection<GenreDtoResponse>>> ListAsync();
    Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(long id);
    Task<BaseResponseGeneric<long>> AddAsync(GenreDtoRequest request);
    Task<BaseResponse> UpdateAsync(long id, GenreDtoRequest request);
    Task<BaseResponse> DeleteAsync(long id);
}