using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface IConcertService
{
    Task<BaseResponsePagination<ConcertDtoResponse>> ListAsync(string? filter, int page, int rows);
    Task<BaseResponseGeneric<long>> AddAsync(ConcertDtoRequest request);
}