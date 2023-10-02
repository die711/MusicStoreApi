using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface IConcertService
{
    Task<BaseResponseGeneric<ConcertDtoResponse>> ListAsync(string? filter, int page, int rows);
}