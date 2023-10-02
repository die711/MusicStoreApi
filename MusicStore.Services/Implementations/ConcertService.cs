using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class ConcertService : IConcertService
{
    private readonly IConcertRepository _repository;
    private readonly ILogger<ConcertService> _logger;
    private readonly IMapper _mapper;

    public ConcertService(IConcertRepository repository, ILogger<ConcertService> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }
    
    
    public async Task<BaseResponsePagination<ConcertDtoResponse>> ListAsync(string? filter, int page, int rows)
    {
        var response = new BaseResponsePagination<ConcertDtoResponse>();

        try
        {
          //  var tupla = await _repository.ListAsync(filter, page, rows);


        }

        catch (Exception ex)
        {
            
            
        }


        return response;

    }

    public async Task<BaseResponseGeneric<long>> AddAsync(ConcertDtoRequest request)
    {
        var response = new BaseResponseGeneric<long>();

        try
        {
            var concert = _mapper.Map<Concert>(request);

            await _repository.AddAsync(concert);
            response.Data = concert.Id;
            response.Success = true;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al agregar el concierto {Message}", ex.Message);
            response.Success = false;
        }

        return response;
    }
}