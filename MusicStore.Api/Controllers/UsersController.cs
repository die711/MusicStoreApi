using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Services.Interfaces;

namespace MusicStore.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDtoRequest request)
    {
        var response = await _service.LoginAsync(request);
        return response.Success ? Ok(response) : StatusCode((int)HttpStatusCode.Unauthorized, response);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDtoRequest request)
    {
        var response = await _service.RegisterAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    
}