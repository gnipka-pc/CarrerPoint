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

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.HashedPassword));
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password));
    }
}
