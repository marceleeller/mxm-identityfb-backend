using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MxmIdentityfbBackend.Domain.Dtos;
using MxmIdentityfbBackend.Domain.Models;
using MxmIdentityfbBackend.Infra.Services;
using Newtonsoft.Json;

namespace MxmIdentityfbBackend.Controllers;
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly AppSettings _applicationSettings;
    private readonly HttpClient _httpClient = new HttpClient();

    public AuthController(AuthService userService, IOptions<AppSettings> applicationSettings, HttpClient httpClient)
    {
        _authService = userService;
        _applicationSettings = applicationSettings.Value;
        _httpClient = httpClient;
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
        HttpResponseMessage debugTokenResponse = await _httpClient
            .GetAsync($"https://graph.facebook.com/debug_token?input_token={credential}&access_token={_applicationSettings.FacebookAppId}|{_applicationSettings.FacebookAppSecret}");

        var stringThing = await debugTokenResponse.Content.ReadAsStringAsync();
        var userOBJ = JsonConvert.DeserializeObject<FBUser>(stringThing);

        if (userOBJ!.Data.IsValid == false)
            return Unauthorized();

        HttpResponseMessage meResponse = await _httpClient
            .GetAsync($"https://graph.facebook.com/me?fields=first_name,last_name,email,id&access_token={credential}");
        var userContent = await meResponse.Content.ReadAsStringAsync();
        var userContentObj = JsonConvert.DeserializeObject<FBUserInfo>(userContent);

        return Ok();
    }
}
