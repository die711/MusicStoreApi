using System.Formats.Tar;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleService _service;
    private readonly ILogger<SalesController> _logger;


    public SalesController(ISaleService service, ILogger<SalesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post(SaleDtoRequest request)
    {
        var email = HttpContext.User.Identity.Name;
        _logger.LogInformation("Autenticado como  {Email}",email);
        _logger.LogInformation("El token vencera el dia {Value}", HttpContext.User.Claims.First(p => p.Type == ClaimTypes.Expiration).Value);
        
        var response = await _service.AddAsync(email, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("ListSalesByDate")]
    public async Task<IActionResult> GetListSalesByDate(string dateStart, string dateEnd, int page = 1, int rows = 10)
    {
        try
        {
            var response = await _service.ListAsync(DateTime.Parse(dateStart), DateTime.Parse(dateEnd), page, rows);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);

        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,"Error en conversion de formato de fecha {message}", ex.Message);
            return BadRequest(new BaseResponse { ErrorMessage = "Error conversion de fecha"});

        }
    }

    [HttpGet("ListSale")]
    public async Task<IActionResult> GetListSales(string? filter, int page = 1, int rows = 10)
    {
        var email = HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;
        var response = await _service.ListAsync(email, filter, page, rows);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetSaleAsync(long id)
    {
        var response = await _service.GetSaleAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }
    
}