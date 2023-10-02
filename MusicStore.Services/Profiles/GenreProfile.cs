using AutoMapper;
using MusicStore.Dto.Response;
using MusicStore.Entities;

namespace MusicStore.Services.Profiles;

public class GenreProfile : Profile
{
    public GenreProfile()
    {
        CreateMap<Genre, GenreDtoResponse>();
        CreateMap<GenreDtoResponse, Genre>();
    }
}