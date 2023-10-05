using AutoMapper;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Entities.Info;

namespace MusicStore.Services.Profiles;

public class ConcertProfile : Profile
{
    public ConcertProfile()
    {
        CreateMap<ConcertInfo, ConcertDtoResponse>();
        CreateMap<Concert, ConcertSingleDtoResponse>()
            .ForMember(dest => dest.Id, orig => orig.MapFrom(p => p.Id))
            .ForMember(dest => dest.Title, orig => orig.MapFrom(p => p.Title))
            .ForMember(dest => dest.Description, orig => orig.MapFrom(p => p.Description))
            .ForMember(dest => dest.TicketQuantity, orig => orig.MapFrom(p => p.TicketsQuantity))
            .ForMember(dest => dest.UnitPrice, orig => orig.MapFrom(p => p.UnitPrice))
            .ForMember(dest => dest.Place, orig => orig.MapFrom(p => p.Place))
            .ForMember(dest => dest.ImageUrl, orig => orig.MapFrom(p => p.ImageUrl))
            .ForMember(dest => dest.DateEvent, orig => orig.MapFrom(p => p.DateEvent.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.TimeEvent, orig => orig.MapFrom(p => p.DateEvent.ToString("HH:mm:ss")))
            .ForMember(dest => dest.GenreDtoResponse, orig => orig.MapFrom(p => p.Genre))
            .ForMember(dest => dest.Status, orig => orig.MapFrom(p => p.Status ? "Activo" : "Inactivo"));

        CreateMap<ConcertDtoRequest, Concert>()
            .ForMember(dest => dest.GenreId, orig => orig.MapFrom(x => x.IdGenre))
            .ForMember(dest => dest.Title, orig => orig.MapFrom(x => x.Title))
            .ForMember(dest => dest.Description, orig => orig.MapFrom(x => x.Description))
            .ForMember(dest => dest.TicketsQuantity, orig => orig.MapFrom(x => x.TicketsQuantity))
            .ForMember(dest => dest.UnitPrice, orig => orig.MapFrom(x => x.UnitPrice))
            .ForMember(dest => dest.Place, orig => orig.MapFrom(x => x.Place))
            .ForMember(dest => dest.DateEvent,
                orig => orig.MapFrom(x => DateTime.Parse($"{x.DateEvent} {x.TimeEvent}")));




    }
}