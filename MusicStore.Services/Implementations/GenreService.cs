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
    private readonly IUnitOfWork _unitOfWork;

    public GenreService(IGenreRepository repository, ILogger<GenreService> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponseGeneric<ICollection<GenreDtoResponse>>> ListAsync()
    {
        var response = new BaseResponseGeneric<ICollection<GenreDtoResponse>>();

        var collection = await _repository.ListAsync(x => x.Status);

        response.Data = _mapper.Map<ICollection<GenreDtoResponse>>(collection);
        response.Success = true;

        return response;
    }

    public async Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(long id)
    {
        var response = new BaseResponseGeneric<GenreDtoResponse>();

        var entity = await _repository.FindByIdAsync(id);

        if (entity == null)
        {
            response.Success = false;
            response.ErrorMessage = "No se encontro el genero";
            return response;
        }

        response.Data = _mapper.Map<GenreDtoResponse>(entity);
        response.Success = true;

        return response;
    }

    public async Task<BaseResponseGeneric<long>> AddAsync(GenreDtoRequest request)
    {
        var response = new BaseResponseGeneric<long>();

        response.Data = await _repository.AddAsync(_mapper.Map<Genre>(request));
        await _unitOfWork.SaveChangesAsync();
        response.Success = true;

        return response;
    }

    public async Task<BaseResponse> UpdateAsync(long id, GenreDtoRequest request)
    {
        var response = new BaseResponse();

        var entity = await _repository.FindByIdAsync(id);

        if (entity == null)
        {
            response.Success = false;
            response.ErrorMessage = "No se encontro el genero";
            return response;
        }

        _mapper.Map(request, entity);
        await _repository.UpdateAsync();
        await _unitOfWork.SaveChangesAsync();
        response.Success = true;

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(long id)
    {
        var response = new BaseResponse();

        await _repository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        response.Success = true;

        return response;

    }
}