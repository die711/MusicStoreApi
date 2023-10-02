using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Response;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;
    private readonly ILogger<GenreService> _logger;
    private readonly IMapper _mapper;

    public GenreService(IGenreRepository genreRepository, ILogger<GenreService> logger, IMapper mapper)
    {
        _genreRepository = genreRepository;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<BaseResponseGeneric<ICollection<GenreDtoResponse>>> ListAsync()
    {
        var response = new BaseResponseGeneric<ICollection<GenreDtoResponse>>();

        try
        {
            var collection = await _genreRepository.ListAsync(x => x.Status);

            response.Data = _mapper.Map<ICollection<GenreDtoResponse>>(collection);
            response.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en listar generos {Message}", ex.Message);
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }

    public Task<BaseResponseGeneric<GenreDtoResponse>> FindByIdAsync(long id)
    {
        throw new NotImplementedException();
    }
}