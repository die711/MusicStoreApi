using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Repositories.Interfaces;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly IGenreService _service;

    public GenreController(IGenreService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync()
    {
        return Ok(await _service.ListAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Genre>> GetById(long id)
    {
        var response = await _service.FindByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Add(GenreDtoRequest request)
    {
        var response = await _service.AddAsync(request);
        return CreatedAtAction(nameof(GetById), new
        {
            id = response.Data,
        }, response);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync(long id, GenreDtoRequest request)
    {
        return Ok(await _service.UpdateAsync(id, request));
    }
    
    
}