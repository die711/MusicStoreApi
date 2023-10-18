using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface ISaleService
{
    Task<BaseResponseGeneric<long>> AddAsync(string email, SaleDtoRequest request);
}