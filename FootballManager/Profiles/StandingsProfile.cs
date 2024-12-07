using AutoMapper;

namespace FootballManager.Profiles
{
    public class StandingsProfile : Profile
    {
        public StandingsProfile()
        {
            CreateMap<Entities.Standing, Models.StandingDto>()
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Wins * 3 + src.Draws))
                .ForMember(dest => dest.GoalDifference, opt => opt.MapFrom(src => src.GoalsFor - src.GoalsAgainst))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team != null ? $"{src.Team.Name}" : "No Team found"));
        }
    }
}
