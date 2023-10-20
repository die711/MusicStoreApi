using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Utils;

namespace MusicStore.Services.Implementations;

public class ConcertService : IConcertService
{
    private readonly IConcertRepository _repository;
    private readonly ILogger<ConcertService> _logger;
    private readonly IMapper _mapper;
    private readonly IFileUploader _fileUploader;

    public ConcertService(IConcertRepository repository, ILogger<ConcertService> logger, IMapper mapper, IFileUploader fileUploader)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _fileUploader = fileUploader;
    }
    
    
    public async Task<BaseResponsePagination<ConcertDtoResponse>> ListAsync(string? filter, int page, int rows)
    {
        var response = new BaseResponsePagination<ConcertDtoResponse>();

        try
        {
            var tuple = await _repository.ListAsync(filter, page, rows);
            response.Data = tuple.Collection.Select(v => _mapper.Map<ConcertDtoResponse>(v)).ToList();
           // response.Data = _mapper.Map<ICollection<ConcertDtoResponse>>(tupla.Collection);
            response.TotalPages = Utilities.GetTotalPages(tuple.Total, rows);
            response.Success = true;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar los conciertos {Message}", ex.Message);
            response.ErrorMessage = "Error al listar los conciertos";
            response.Success = false;
        }
        
        return response;
    }

    
    public async Task<BaseResponseGeneric<ConcertSingleDtoResponse>> FindByIdAsync(long id)
    {
        var response = new BaseResponseGeneric<ConcertSingleDtoResponse>();
        try
        {
            var concert =await _repository.FindByIdAsync(id);

            if (concert is null)
            {
                response.ErrorMessage = $"No se encontro el concierto con id {id}";
                return response;
            }

            response.Data = _mapper.Map<ConcertSingleDtoResponse>(concert);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el concierto");
            response.ErrorMessage = "Error al obtener el concierto";
        }

        return response;
    }

    public async Task<BaseResponseGeneric<long>> AddAsync(ConcertDtoRequest request)
    {
        var response = new BaseResponseGeneric<long>();

        try
        {
            var concert = _mapper.Map<Concert>(request);
            concert.ImageUrl = await _fileUploader.UploadFileAsync(request.Base64Image, request.FileName);
            
            await _repository.AddAsync(concert);
            response.Data = concert.Id;
            response.Success = true;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error al agregar el concierto {Message}", ex.Message);
            response.ErrorMessage = "Error al agregar el concierto";
        }

        return response;
    }


    public async Task<BaseResponse> UpdateAsync(long id, ConcertDtoRequest request)
    {
        var response = new BaseResponse();

        try
        {
            var concert = await _repository.FindByIdAsync(id);

            if (concert == null)
            {
                response.ErrorMessage = "No se encontro el concierto";
                return response;
            }

            _mapper.Map(request, concert);

            if (!string.IsNullOrEmpty(request.FileName))
                concert.ImageUrl = await _fileUploader.UploadFileAsync(request.Base64Image, request.FileName);
            
            await _repository.UpdateAsync();
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el concierto {Message}", ex.Message);
            response.ErrorMessage = "Error al actualizar el concierto";
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
            _logger.LogError(ex,"Error al eliminar el concierto con el id {id}", id);
            response.ErrorMessage = "Error al eliminar el concierto";
        }

        return response;
    }

    public async Task<BaseResponse> FinalizeAsync(long id)
    {
        var response = new BaseResponse();

        try
        {
            await _repository.FinalizeAsync(id);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = _logger.LogMessage(ex, nameof(FinalizeAsync));
        }

        return response;

    }
    
    
}