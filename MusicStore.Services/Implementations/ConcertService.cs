using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.DataAccess;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Utils;

namespace MusicStore.Services.Implementations;

public class ConcertService : IConcertService
{
    private readonly MusicStoreDbContext _context;
    private readonly ILogger<ConcertService> _logger;
    private readonly IMapper _mapper;
    private readonly IFileUploader _fileUploader;

    public ConcertService(MusicStoreDbContext context, ILogger<ConcertService> logger, IMapper mapper, IFileUploader fileUploader)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
        _fileUploader = fileUploader;
    }


    public async Task<BaseResponsePagination<ConcertDtoResponse>> ListAsync(string? filter, int page, int rows)
    {
        var response = new BaseResponsePagination<ConcertDtoResponse>();

        var query = _context.Set<Concert>()
            .Where(p => p.Status && p.Title.Contains(filter ?? string.Empty));

        var collection = await query
            .OrderBy(p => p.Title)
            .Skip((page - 1) * rows)
            .Take(rows)
            .AsNoTracking()
            .Select(v => _mapper.Map<ConcertDtoResponse>(v))
            .ToListAsync();

        var total = await query.CountAsync();

        response.Data = collection;
        response.TotalPages = Utilities.GetTotalPages(total, rows);
        response.Success = true;
        return response;
    }


    public async Task<BaseResponseGeneric<ConcertSingleDtoResponse>> FindByIdAsync(long id)
    {
        var response = new BaseResponseGeneric<ConcertSingleDtoResponse>();
        var concert = await _context.Set<Concert>()
            .Include(p => p.Genre)
            .FirstOrDefaultAsync(x => x.Id == id);

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

        await _context.Set<Concert>().AddAsync(concert);
        await _context.SaveChangesAsync();
        response.Data = concert.Id;
        response.Success = true;

        return response;
    }


    public async Task<BaseResponse> UpdateAsync(long id, ConcertDtoRequest request)
    {
        var response = new BaseResponse();

        var concert = await _context.Set<Concert>().FindAsync(id);

        if (concert == null)
        {
            response.ErrorMessage = "No se encontro el concierto";
            return response;
        }

        _mapper.Map(request, concert);

        if (!string.IsNullOrEmpty(request.FileName))
            concert.ImageUrl = await _fileUploader.UploadFileAsync(request.Base64Image, request.FileName);

        await _context.SaveChangesAsync();
        response.Success = true;

        return response;

    }

    public async Task<BaseResponse> DeleteAsync(long id)
    {
        var response = new BaseResponse();
        var entity = await _context.Set<Concert>().FindAsync(id);
        if (entity != null)
        {
            entity.Status = false;
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException($"No se encontro el registro con el Id {id}");
        }
        response.Success = true;

        return response;
    }

    public async Task<BaseResponse> FinalizeAsync(long id)
    {
        var response = new BaseResponse();

        var entity = await _context.Set<Concert>().FindAsync(id);
        if (entity is not null)
        {
            entity.Finalized = true;
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException($"Unable to finalize entity {id}");
        }
        response.Success = true;

        return response;

    }


}