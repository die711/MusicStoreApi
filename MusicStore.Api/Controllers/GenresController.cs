using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GenresController : ControllerBase
{
    private readonly IGenreService _service;

    public GenresController(IGenreService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListAsync()
    {
        return Ok(await _service.ListAsync());
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<Genre>> GetById(long id)
    {
        var response = await _service.FindByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Add(GenreDtoRequest request)
    {
        var response = await _service.AddAsync(request);
        return response.Success ? CreatedAtAction(nameof(GetById), new
        {
            id = response.Data,
        }, response) : BadRequest(response);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync(long id, GenreDtoRequest request)
    {
        var response = await _service.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var response = await _service.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }


}