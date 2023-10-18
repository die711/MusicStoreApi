using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
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
    
    
}