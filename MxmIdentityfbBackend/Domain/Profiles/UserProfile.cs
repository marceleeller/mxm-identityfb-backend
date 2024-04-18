using AutoMapper;
using MxmIdentityfbBackend.Domain.Dtos;
using MxmIdentityfbBackend.Domain.Models;

namespace MxmIdentityfbBackend.Domain.Profiles;

public class UserProfile : Profile
{
    protected UserProfile()
    {
        CreateMap<UserRegisterDto, User>();
    }
    
}
