using AutoMapper;

namespace FootballManager.Profiles
{
    public class GamesProfile : Profile
    {
        public GamesProfile()
        {
            CreateMap<Entities.Game, Models.GameDto>()
                .ForMember(dest => dest.HomeTeamName, opt => opt.MapFrom(src => $"{src.HomeTeam.Name}"))
                .ForMember(dest => dest.AwayTeamName, opt => opt.MapFrom(src => $"{src.AwayTeam.Name}"))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => $"{src.HomeTeamScore} - {src.AwayTeamScore}"))
                .ForMember(dest => dest.SeasonYear, opt => opt.MapFrom(src => $"{src.LeagueYear}"));
            CreateMap<Models.GameForCreationDto, Entities.Game>();
            CreateMap<Models.GameForUpdateDto, Entities.Game>();
            CreateMap<Entities.Game, Models.GameForUpdateDto>();
        }
    }
}
