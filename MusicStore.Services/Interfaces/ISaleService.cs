using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface ISaleService
{
    Task<BaseResponseGeneric<long>> AddAsync(string email, SaleDtoRequest request);
    Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(DateTime dateStart, DateTime dateEnd, int page, int rows);
    Task<BaseResponsePagination<SaleDtoResponse>> ListAsync(string email, string? filter, int page, int rows);
    Task<BaseResponseGeneric<SaleDtoResponse>> GetSaleAsync(long id);

}