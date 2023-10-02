using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ConcertController : ControllerBase
{
    private readonly IConcertService _service;

    public ConcertController(IConcertService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync(string? filter, int page = 1, int rows = 10)
    {
        return Ok(await _service.ListAsync(filter, page, rows));
    }

    
    // [HttpPost]
    // public async Task<IActionResult> Add(ConcertDtoRequest request)
    // {
    //     var response = await _service.AddAsync(request);
    //     return CreatedAtAction(nameof(GetById), new
    //     {
    //         id = response.Data,
    //     }, response);
    // }

   


}