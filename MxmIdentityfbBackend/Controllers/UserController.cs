using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MxmIdentityfbBackend.Domain.Dtos;
using MxmIdentityfbBackend.Domain.Models;

namespace MxmIdentityfbBackend.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(AuthenticationSchemes = "Bearer")]

public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserController(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Retorna o usuário autenticado.
    /// </summary>
    /// <response code="200">Dados pessoais do usuario</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpGet]
    [ProducesResponseType(type: typeof(UserResponseDto), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(type: typeof(string), statusCode: StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUser()
    {
        var accountIdClaim = User.FindFirst("Id");
        if(accountIdClaim == null)
            return Unauthorized("Usuário não autenticado");

        var user = await _userManager.FindByIdAsync(accountIdClaim.Value);
        var response = _mapper.Map<UserResponseDto>(user);
        return Ok(response);
    }
    
}
