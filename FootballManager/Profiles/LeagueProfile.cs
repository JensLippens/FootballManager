using AutoMapper;

namespace FootballManager.Profiles
{
    public class LeagueProfile : Profile
    {
        public LeagueProfile()
        {
            CreateMap<Entities.League, Models.LeagueWithTeamsAndGamesDto>();
            CreateMap<Entities.League, Models.LeagueWithGamesDto>();
            CreateMap<Entities.League, Models.LeagueWithTeamsDto>();
            CreateMap<Entities.League, Models.LeagueDto>();
            CreateMap<Models.LeagueForCreationDto, Entities.League>();
        }
    }
}
