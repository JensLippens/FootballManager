using AutoMapper;

namespace FootballManager.Profiles
{
    public class CoachProfile : Profile
    {
        public CoachProfile()
        {
            CreateMap<Entities.Coach, Models.CoachDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src =>
                    DateTime.Today.Year - src.DateOfBirth.Year
                    - (DateTime.Today < src.DateOfBirth.AddYears(DateTime.Today.Year - src.DateOfBirth.Year) ? 1 : 0)));
            CreateMap<Models.CoachForCreationDto, Entities.Coach>();
            CreateMap<Models.CoachForUpdateDto, Entities.Coach>();
            CreateMap<Entities.Coach, Models.CoachForUpdateDto>();
        }
    }
}
