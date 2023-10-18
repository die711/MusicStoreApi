using AutoMapper;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;

namespace MusicStore.Services.Profiles;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SaleDtoRequest, Sale>()
            .ForMember(dest => dest.ConcertId, orig => orig.MapFrom(x => x.ConcertId))
            .ForMember(dest => dest.Quantity, orig => orig.MapFrom(x => x.TicketQuantity));

        CreateMap<Sale, SaleDtoResponse>()
            .ForMember(dest => dest.SaleId, orig => orig.MapFrom(x => x.Id))
            .ForMember(dest => dest.DateEvent, orig => orig.MapFrom(x => x.Concert.DateEvent.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.TimeEvent, orig => orig.MapFrom(x => x.Concert.DateEvent.ToString("HH:mm:ss")))
            .ForMember(dest => dest.Genre, orig => orig.MapFrom(x => x.Concert.Genre.Name))
            .ForMember(dest => dest.ImageUrl, orig => orig.MapFrom(x => x.Concert.ImageUrl))
            .ForMember(dest => dest.Title, orig => orig.MapFrom(x => x.Concert.Title))
            .ForMember(dest => dest.OperationNumber, orig => orig.MapFrom(x => x.OperationNumber))
            .ForMember(dest => dest.FullName, orig => orig.MapFrom(x => x.Customer.FullName))
            .ForMember(dest => dest.Quantity, orig => orig.MapFrom(x => x.Quantity))
            .ForMember(dest => dest.SaleDate, orig => orig.MapFrom(x => x.SaleDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.Total, orig => orig.MapFrom(x => x.Total));



    }   
}