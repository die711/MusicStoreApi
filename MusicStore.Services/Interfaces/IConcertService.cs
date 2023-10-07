using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface IConcertService
{
    Task<BaseResponsePagination<ConcertDtoResponse>> ListAsync(string? filter, int page, int rows);
    Task<BaseResponseGeneric<ConcertSingleDtoResponse>> FindByIdAsync(long id);
    Task<BaseResponseGeneric<long>> AddAsync(ConcertDtoRequest request);
    Task<BaseResponse> UpdateAsync(long id, ConcertDtoRequest request);
    Task<BaseResponse> FinalizeAsync(long id);
    Task<BaseResponse> DeleteAsync(long id);
}