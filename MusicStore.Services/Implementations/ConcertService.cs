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
    private readonly IUnitOfWork _unitOfWork;

    public ConcertService(IConcertRepository repository, ILogger<ConcertService> logger, IMapper mapper, IFileUploader fileUploader, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _fileUploader = fileUploader;
        _unitOfWork = unitOfWork;
    }


    public async Task<BaseResponsePagination<ConcertDtoResponse>> ListAsync(string? filter, int page, int rows)
    {
        var response = new BaseResponsePagination<ConcertDtoResponse>();

        var tuple = await _repository.ListAsync(filter, page, rows);
        response.Data = tuple.Collection.Select(v => _mapper.Map<ConcertDtoResponse>(v)).ToList();
        // response.Data = _mapper.Map<ICollection<ConcertDtoResponse>>(tupla.Collection);
        response.TotalPages = Utilities.GetTotalPages(tuple.Total, rows);
        response.Success = true;
        return response;
    }


    public async Task<BaseResponseGeneric<ConcertSingleDtoResponse>> FindByIdAsync(long id)
    {
        var response = new BaseResponseGeneric<ConcertSingleDtoResponse>();
        var concert = await _repository.FindByIdAsync(id);

        if (concert is null)
        {
            response.ErrorMessage = $"No se encontro el concierto con id {id}";
            return response;
        }

        response.Data = _mapper.Map<ConcertSingleDtoResponse>(concert);
        response.Success = true;

        return response;
    }

    public async Task<BaseResponseGeneric<long>> AddAsync(ConcertDtoRequest request)
    {
        var response = new BaseResponseGeneric<long>();

        var concert = _mapper.Map<Concert>(request);
        concert.ImageUrl = await _fileUploader.UploadFileAsync(request.Base64Image, request.FileName);

        await _repository.AddAsync(concert);
        await _unitOfWork.SaveChangesAsync();
        response.Data = concert.Id;
        response.Success = true;

        return response;
    }


    public async Task<BaseResponse> UpdateAsync(long id, ConcertDtoRequest request)
    {
        var response = new BaseResponse();

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

    public async Task<BaseResponse> FinalizeAsync(long id)
    {
        var response = new BaseResponse();

        await _repository.FinalizeAsync(id);
        await _unitOfWork.SaveChangesAsync();
        response.Success = true;

        return response;

    }


}