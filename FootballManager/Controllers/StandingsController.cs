using AutoMapper;
using FootballManager.Models;
using FootballManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [ApiController]
    [Route("api/standings")]
    public class StandingsController : ControllerBase
    {
        private readonly ILogger<StandingsController> _logger;
        private readonly IFootballManagerRepository _repo;
        private readonly IMapper _mapper;

        public StandingsController(ILogger<StandingsController> logger, IFootballManagerRepository repo, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("current")]
        public async Task<ActionResult<IEnumerable<StandingDto>>> GetCurrentLeagueStanding()
        {
            var standingEntities = await _repo.GetCurrentLeagueStandingsAsync();

            var standingDtos = _mapper.Map<IEnumerable<StandingDto>>(standingEntities)
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference);

            return Ok(standingDtos);
        }

        [HttpGet("league/{leagueYear}")]
        public async Task<ActionResult<IEnumerable<StandingDto>>> GetLeagueYearStanding(int leagueYear)
        {
            var standingEntities = await _repo.GetLeagueYearStandingsAsync(leagueYear);

            var standingDtos = _mapper.Map<IEnumerable<StandingDto>>(standingEntities)
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference);

            return Ok(standingDtos);
        }

    }
}
