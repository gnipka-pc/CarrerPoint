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

        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
    }
}
