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

public class GenreService : IGenreService
{
    private readonly MusicStoreDbContext _context;
    private readonly ILogger<GenreService> _logger;
    private readonly IMapper _mapper;

    public GenreService(MusicStoreDbContext context, ILogger<GenreService> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BaseResponseGeneric<ICollection<GenreDtoResponse>>> ListAsync()
    {
        var response = new BaseResponseGeneric<ICollection<GenreDtoResponse>>();

        var collection = await _context.Set<Genre>()
            .Where(x => x.Status)
            .AsNoTracking()
            .ToListAsync();

        response.Data = _mapper.Map<ICollection<GenreDtoResponse>>(collection);
        response.Success = true;

        return response;
    }

    public async Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(long id)
    {
        var response = new BaseResponseGeneric<GenreDtoResponse>();

        var entity = await _context.Set<Genre>().FindAsync(id);

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

        var entity = _mapper.Map<Genre>(request);
        await _context.Set<Genre>().AddAsync(entity);
        await _context.SaveChangesAsync();

        response.Data = entity.Id;
        response.Success = true;

        return response;
    }

    public async Task<BaseResponse> UpdateAsync(long id, GenreDtoRequest request)
    {
        var response = new BaseResponse();

        var entity = await _context.Set<Genre>().FindAsync(id);

        if (entity == null)
        {
            response.Success = false;
            response.ErrorMessage = "No se encontro el genero";
            return response;
        }

        _mapper.Map(request, entity);
        await _context.SaveChangesAsync();
        response.Success = true;

        return response;
    }

    public async Task<BaseResponse> DeleteAsync(long id)
    {
        var response = new BaseResponse();

        var entity = await _context.Set<Genre>().FindAsync(id);

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
}