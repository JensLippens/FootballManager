using AutoMapper;

namespace FootballManager.Profiles
{
    public class TeamsProfile : Profile
    {
        public TeamsProfile()
        {
            CreateMap<Entities.Team, Models.TeamDto>();
            CreateMap<Entities.Team, Models.TeamWithoutGamesOrPlayersDto>();
            CreateMap<Models.TeamForCreationDto, Entities.Team>();
            CreateMap<Models.TeamForUpdateDto, Entities.Team>();
            CreateMap<Entities.Team, Models.TeamForUpdateDto>();
        }
    }
}
