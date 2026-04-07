using AutoMapper;
using CareerPoint.Infrastructure.DTOs;
using CareerPoint.Infrastructure.Model;

namespace CareerPoint.Application.Mappings;

public class CareerPointProfile : Profile
{
    public CareerPointProfile()
    {
        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.Tags.ToDictionary(t => t.Key, t => t.Value)));

        CreateMap<EventDto, Event>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.Tags.Select(kvp => new EventTag { Key = kvp.Key, Value = kvp.Value }).ToList()));

        CreateMap<CreateEventDto, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Users, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.Tags.Select(kvp => new EventTag { Key = kvp.Key, Value = kvp.Value }).ToList()));


        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.HashedPassword, opt => opt.MapFrom(src => src.Password));

        CreateMap<UpdateUserDto, User>();
    }
}
