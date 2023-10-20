using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Utils;

namespace MusicStore.Api.Endpoints;

public static class ReportEndpoints
{
    public static void MapReports(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/Reports").WithTags("Reports");

        group.MapGet("/", async (ISaleService service, string dateStart, string dateEnd, ILogger<Program> logger) =>
        {
            try
            {
                var response = await service.GetReportSaleAsync(DateTime.Parse(dateStart), DateTime.Parse(dateEnd));

                if (response.Success)
                    return Results.Ok(response);

                return Results.BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new BaseResponse()
                {
                    ErrorMessage = logger.LogMessage(ex, "Reportes")
                };
                return Results.BadRequest(response);
            }
        });
    }
}