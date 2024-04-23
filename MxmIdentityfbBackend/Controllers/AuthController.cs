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
    /// Registrar novo usuario.
    /// </summary>
    /// <response code="201">Usuario registrado com sucesso</response>
    /// <response code="400">Falha ao registrar usuário</response>
    [HttpPost("register")]
    [ProducesResponseType(type: typeof(User), statusCode: StatusCodes.Status201Created)]
    [ProducesResponseType(type: typeof(string), statusCode: StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        try
        {
            var user = await _authService.Register(userRegisterDto);

            return CreatedAtAction(
                nameof(UserController.GetUser), "User",
                new { id = user.Id },
                new { user, message = "Usuario registrado com sucesso" }
            );

        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

    /// <summary>
    /// Realizar o login de um usuário.
    /// </summary>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="401">Credenciais inválidas</response>
    [HttpPost("login")]
    [ProducesResponseType(type: typeof(UserTokenResponseDto), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(type: typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        try
        {
            var tokenResponse = await _authService.Login(userLoginDto);

            return Ok(new { tokenResponse, message = "Login realizado com sucesso" });

        } catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
        
    }

    /// <summary>
    /// Realizar o login de um usuário usando Facebook
    /// </summary>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="401">Falha ao autenticar usuário</response>
    [HttpPost("loginWithFacebook")]
    public async Task<IActionResult> LoginWithFacebook([FromBody] string credential)
    {

        try
        {
            var tokenResponse = await _authService.LoginWithFacebook(credential);

            return Ok(new { tokenResponse, message = "Login realizado com sucesso" });
        } catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
               
    }

    /// <summary>
    /// Realizar o login de um usuário usando Google
    /// </summary>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="401">Falha ao autenticar usuário</response>
    [HttpPost("loginWithGoogle")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] string credential)
    {
        try
        {
            var tokenResponse = await _authService.LoginWithGoogle(credential);

            return Ok(new { tokenResponse, message = "Login realizado com sucesso" });
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }
}
