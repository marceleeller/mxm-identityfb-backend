using AutoMapper;
using MxmIdentityfbBackend.Domain.Dtos;
using MxmIdentityfbBackend.Domain.Models;

namespace MxmIdentityfbBackend.Domain.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRegisterDto, User>();
        CreateMap<User, UserResponseDto>();
    }
    
}
