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

    /// <summary>
    /// Register new user.
    /// </summary>
    /// <response code="201">Client successfully registered</response>
    /// <response code="400">Failed to register user</response>
    [HttpPost("register")]
    [ProducesResponseType(type: typeof(User), statusCode: StatusCodes.Status201Created)]
    [ProducesResponseType(type: typeof(string), statusCode: StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        try
        {
            var user = await _authService.Register(userRegisterDto);

            return CreatedAtAction(
                nameof(UserController.GetUser), "Data",
                new { id = user.Id },
                new { user, message = "Client successfully registered" }
            );

        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

    /// <summary>
    /// Authenticate user.
    /// </summary>
    /// <response code="200">Login successful</response>
    /// <response code="401">Failed to login</response>
    [HttpPost("login")]
    [ProducesResponseType(type: typeof(UserTokenResponseDto), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(type: typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        try
        {
            var tokenResponse = await _authService.Login(userLoginDto);

            return Ok(new { tokenResponse, message = "Login successful" });

        } catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
        
    }

    /// <summary>
    /// Authenticate user with Facebook.
    /// </summary>
    /// <response code="200">Login successful</response>
    /// <response code="401">Failed to login</response>
    [HttpPost("loginWithFacebook")]
    public async Task<IActionResult> LoginWithFacebook([FromBody] string credential)
    {

        try
        {
            var tokenResponse = await _authService.LoginWithFacebook(credential);

            return Ok(new { tokenResponse, message = "Login successful" });
        } catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
               
    }
}
