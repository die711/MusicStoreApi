using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ConcertsController : ControllerBase
{
    private readonly IConcertService _service;

    public ConcertsController(IConcertService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync(string? filter, int page = 1, int rows = 10)
    {
        return Ok(await _service.ListAsync(filter, page, rows));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> FindById(long id)
    {
        var response = await _service.FindByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(ConcertDtoRequest request)
    {
        var response = await _service.AddAsync(request);
        return CreatedAtAction(nameof(FindById), new
        {
            id = response.Data,
        }, response);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put(long id, ConcertDtoRequest request)
    {
        var response = await _service.UpdateAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var response = await _service.DeleteAsync(id);
        return Ok(response);
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Patch(long id)
    {
        var response = await _service.FinalizeAsync(id);
        return Ok(response);
    }
    
    
    
   


}