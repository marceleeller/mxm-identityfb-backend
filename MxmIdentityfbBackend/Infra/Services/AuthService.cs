using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MxmIdentityfbBackend.Domain.Dtos;
using MxmIdentityfbBackend.Domain.Models;

namespace MxmIdentityfbBackend.Infra.Services;

public class AuthService
{

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly TokenService _tokenService;

    public AuthService(UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager, TokenService tokenService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<UserTokenResponseDto> Login(UserLoginDto userLoginDto)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(userLoginDto.UserName, userLoginDto.Password, false, false);

        if(!signInResult.Succeeded)
            throw new Exception("Falha ao logar usuario");

        var user = await _userManager.Users.FirstAsync(user => user.NormalizedUserName.Equals(userLoginDto.UserName.ToUpper()));

        return new UserTokenResponseDto()
        {
            Token = _tokenService.GenerateToken(user)
        };
    }

    public async Task<User> Register(UserRegisterDto userRegisterDto)
    {

        var user = _mapper.Map<User>(userRegisterDto);
        var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

        if (!result.Succeeded)
            throw new Exception("Falha ao cadastrar usuario");

        return user;
    }



}
