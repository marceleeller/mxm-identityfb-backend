using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MxmIdentityfbBackend.Domain.Dtos;
using MxmIdentityfbBackend.Domain.Models;
using MxmIdentityfbBackend.Helpers;
using MxmIdentityfbBackend.Infra.Services;
using Newtonsoft.Json;
using System.Net.Http;

namespace MxmIdentityfbBackend.Controllers;
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;


    public AuthController(AuthService userService)
    {
        _authService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        var user = await _authService.Register(userRegisterDto);

        return Created($"/api/users/{user.Id}", user);

    }

    [HttpPost("login")]
    [ProducesResponseType(type: typeof(UserTokenResponseDto), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        var tokenResponse = await _authService.Login(userLoginDto);

        return Ok(tokenResponse);
    }

    [HttpPost("loginWithFacebook")]
    public async Task<IActionResult> LoginWithFacebook([FromBody] string credential)
    {
        var tokenResponse = await _authService.LoginWithFacebook(credential);
       
        return Ok(tokenResponse);
               
    }
}
