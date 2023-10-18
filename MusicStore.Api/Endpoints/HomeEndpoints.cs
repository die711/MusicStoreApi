using MusicStore.Services.Implementations;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Endpoints;

public static class HomeEndpoints
{
    public static void MapHomeEndPoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/Home").WithTags("Home");

        group.MapGet("/", async (IConcertService concertService, IGenreService genreService) =>
            {
                var concerts = await concertService.ListAsync(string.Empty, 1, 100);
                var genres = await genreService.ListAsync();

                return Results.Ok(new
                {
                    Concerts = concerts,
                    Genres = genres,
                    Success = true
                });
            }).WithDescription("Permite mostrar los endpoints de la pagina principal")
            .WithOpenApi();
    }
}