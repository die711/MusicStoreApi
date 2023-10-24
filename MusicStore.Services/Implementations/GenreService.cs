using AutoMapper;
using Azure;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Utils;

namespace MusicStore.Services.Implementations;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _repository;
    private readonly ILogger<GenreService> _logger;
    private readonly IMapper _mapper;

    public GenreService(IGenreRepository repository, ILogger<GenreService> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<BaseResponseGeneric<ICollection<GenreDtoResponse>>> ListAsync()
    {
        var response = new BaseResponseGeneric<ICollection<GenreDtoResponse>>();

        try
        {
            var collection = await _repository.ListAsync(x => x.Status);

            response.Data = _mapper.Map<ICollection<GenreDtoResponse>>(collection);
            response.Success = true;
            throw new Exception();
        }
        catch (Exception ex)
        {
            response.ErrorMessage = _logger.LogMessage(ex, nameof(ListAsync));
            _logger.LogError(ex, "Error en listar generos {Message}", ex.Message);
            response.ErrorMessage = ex.Message;
        }

        return response;
    }

    public async Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(long id)
    {
        var response = new BaseResponseGeneric<GenreDtoResponse>();

        try
        {
            var entity = await _repository.FindByIdAsync(id);

            if (entity == null)
            {
                response.Success = false;
                response.ErrorMessage = "No se encontro el genero";
                return response;
            }
            
            response.Data = _mapper.Map<GenreDtoResponse>(entity);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error buscando genero con id {Message} ", ex.Message);
            response.ErrorMessage = "Error al buscar un genero";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<long>> AddAsync(GenreDtoRequest request)
    {
        var response = new BaseResponseGeneric<long>();

        try
        {
            response.Data = await _repository.AddAsync(_mapper.Map<Genre>(request));
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error en generos {Message}", ex.Message);
            response.ErrorMessage = ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse> UpdateAsync(long id, GenreDtoRequest request)
    {
        var response = new BaseResponse();

        try
        {
            var entity = await _repository.FindByIdAsync(id);

            if (entity == null)
            {
                response.Success = false;
                response.ErrorMessage = "No se encontro el genero";
                return response;
            }

            _mapper.Map(request, entity);
            await _repository.UpdateAsync();
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Error en Generos {Message}", ex.Message);
            response.ErrorMessage = ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(long id)
    {
        var response = new BaseResponse();

        try
        {
            await _repository.DeleteAsync(id);
            response.Success = true;

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error al eliminar generos {Message}", ex.Message);
            response.ErrorMessage = ex.Message;
        }

        return response;

    }
}