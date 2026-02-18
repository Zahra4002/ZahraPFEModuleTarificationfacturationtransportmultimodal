using Application.Features.TestFeature.Commands;
using Application.Features.TestFeature.Dtos;
using Application.Features.UserFeature.Dtos;
using AutoMapper;
using Domain.Common;
using Domain.Entities;

namespace Application.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Commands
            CreateMap<AddTestCommandNew, Test>();
            CreateMap<PagedList<Test>, PagedList<TestDTO>>().ReverseMap();

            //CreateMap<AddUserCommandNew, User>();
            CreateMap<PagedList<User>, PagedList<UserDTO>>().ReverseMap();

            //Dto
            CreateMap<Test, TestDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();

        }
    }
}
