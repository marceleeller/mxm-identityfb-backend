﻿using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MxmIdentityfbBackend.Domain.Dtos;
using MxmIdentityfbBackend.Domain.Models;
using MxmIdentityfbBackend.Helpers;
using Newtonsoft.Json;

namespace MxmIdentityfbBackend.Infra.Services;

public class AuthService
{

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly TokenService _tokenService;
    private readonly RandomPasswordService _randomPasswordService;
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly AppSettings _applicationSettings;

    public AuthService(UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager, TokenService tokenService, HttpClient httpClient, IOptions<AppSettings> applicationSettings, RandomPasswordService randomPasswordService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _httpClient = httpClient;
        _applicationSettings = applicationSettings.Value;
        _randomPasswordService = randomPasswordService;
    }

    public async Task<UserTokenResponseDto> Login(UserLoginDto userLoginDto)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(userLoginDto.Email, userLoginDto.Password, false, false);

        if (!signInResult.Succeeded)
            throw new Exception("Credenciais inválidas.");
            
        var user = await _userManager.Users.FirstAsync(user => user.NormalizedEmail.Equals(userLoginDto.Email.ToUpper()));

        return new UserTokenResponseDto()
        {
            Token = _tokenService.GenerateToken(user)
        };
    }

    public async Task<UserTokenResponseDto> LoginWithFacebook(string credential)
    {
        HttpResponseMessage debugTokenResponse = await _httpClient
          .GetAsync("https://graph.facebook.com/debug_token?input_token=" + credential + $"&access_token={_applicationSettings.FacebookAppId}|{_applicationSettings.FacebookAppSecret}");

        var stringThing = await debugTokenResponse.Content.ReadAsStringAsync();
        var userOBJK = JsonConvert.DeserializeObject<FBUser>(stringThing);

        //check if access token is valid
        if (userOBJK.Data.IsValid == false)
            throw new Exception("Falha ao autenticar usuário.");

        //get user info
        HttpResponseMessage meResponse = await _httpClient
            .GetAsync("https://graph.facebook.com/me?fields=first_name,last_name,email,id&access_token=" + credential);
        var userContent = await meResponse.Content.ReadAsStringAsync();
        var userContentObj = JsonConvert.DeserializeObject<FBUserInfo>(userContent);

        //get user from db
        var user = await _userManager.FindByEmailAsync(userContentObj.Email);
        if (user == null)
        {
            var randomPassword = _randomPasswordService.GenerateRandomPassword();
            var userRegister = new UserRegisterDto { Email = userContentObj.Email, FirstName = userContentObj.first_name, LastName = userContentObj.last_name, Password = randomPassword, PasswordConfirmation = randomPassword };
            user = _mapper.Map<User>(userRegister);
            user.UserName = userRegister.Email;

            // register user
            await _userManager.CreateAsync(user, randomPassword);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        return new UserTokenResponseDto()
        {
            Token = _tokenService.GenerateToken(user)
        };

    }

    public async Task<UserTokenResponseDto> LoginWithGoogle(string credential)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { this._applicationSettings.GoogleClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user == null)
        {
            var randomPassword = _randomPasswordService.GenerateRandomPassword();
            var userRegister = new UserRegisterDto { Email = payload.Email, FirstName = payload.GivenName, LastName = payload.FamilyName, Password = randomPassword, PasswordConfirmation = randomPassword };
            user = _mapper.Map<User>(userRegister);
            user.UserName = userRegister.Email;

            // register user
            await _userManager.CreateAsync(user, randomPassword);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        return new UserTokenResponseDto()
        {
            Token = _tokenService.GenerateToken(user)
        };
    }

    public async Task<User> Register(UserRegisterDto userRegisterDto)
    {

        var user = _mapper.Map<User>(userRegisterDto);
        user.UserName = userRegisterDto.Email;
        var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

        if (!result.Succeeded)
        {
            throw new Exception("Falha ao registrar usuário");
        }

        return user;
    }



}
