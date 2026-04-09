using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Application.Mappings;

public class CareerPointProfile : Profile
{
    public CareerPointProfile()
    {
        CreateMap<Event, EventDto>();

        CreateMap<EventDto, Event>();

        CreateMap<CreateEventDto, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Users, opt => opt.Ignore());


        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password));

        CreateMap<UpdateUserDto, User>();
    }
}
