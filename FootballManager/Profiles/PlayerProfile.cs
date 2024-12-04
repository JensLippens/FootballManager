using AutoMapper;

namespace FootballManager.Profiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile() 
        { 
            CreateMap<Entities.Player, Models.PlayerDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src =>
                    DateTime.Today.Year - src.DateOfBirth.Year
                    - (DateTime.Today < src.DateOfBirth.AddYears(DateTime.Today.Year - src.DateOfBirth.Year) ? 1 : 0)));
            CreateMap<Models.PlayerForCreationDto, Entities.Player>();
            CreateMap<Models.PlayerForUpdateDto, Entities.Player>();
            CreateMap<Entities.Player, Models.PlayerForUpdateDto>();
        }
    }
}
