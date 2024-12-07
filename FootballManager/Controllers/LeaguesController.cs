using AutoMapper;
using FootballManager.Models;
using FootballManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [ApiController]
    [Route("api/leagues")]
    public class LeaguesController : ControllerBase
    {
        private readonly ILogger<LeaguesController> _logger;
        private readonly IFootballManagerRepository _repo;
        private readonly IMapper _mapper;

        public LeaguesController(ILogger<LeaguesController> logger, IFootballManagerRepository repo, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeagueDto>>> GetLeagues()
        {
            var leagueEntities = await _repo.GetAllLeaguesAsync();

            return Ok(_mapper.Map<IEnumerable<LeagueDto>>(leagueEntities));
        }

        [HttpGet("league/{leagueYear}", Name = "GetLeague")]
        public async Task<IActionResult> GetLeague(
            int leagueYear, bool includeGames = false, bool includeTeams = false)
        {
            var leagueEntity = await _repo.GetLeagueAsync(leagueYear, includeGames, includeTeams);
            if (leagueEntity == null)
            {
                return NotFound();
            }

            if (includeGames && includeTeams)
            {
                return Ok(_mapper.Map<LeagueWithTeamsAndGamesDto>(leagueEntity));
            }
            else if (includeGames && !includeTeams)
            {
                return Ok(_mapper.Map<LeagueWithGamesDto>(leagueEntity));
            }
            else if (!includeGames && includeTeams)
            {
                return Ok(_mapper.Map<LeagueWithTeamsDto>(leagueEntity));
            }
            else  // if (!includeGames && !includeTeams)
            {
                return Ok(_mapper.Map<LeagueDto>(leagueEntity));
            }
        }

        [HttpPost]
        public async Task<ActionResult<LeagueDto>> NewLeague(LeagueForCreationDto league)
        {
            if (await _repo.LeagueYearExistsAsync(league.Year))
            {
                return BadRequest($"League {league.Year} already exists");
            }
            var leagueToAdd = _mapper.Map<Entities.League>(league);

            await _repo.AddLeagueAsync(leagueToAdd);
            await _repo.SaveChangesAsync();

            var leagueToReturn = _mapper.Map<Models.LeagueDto>(leagueToAdd);

            return CreatedAtRoute("GetLeague",
                new
                {
                    leagueYear = leagueToReturn.Year
                },
                leagueToReturn);
        }

        [HttpDelete("league/{leagueYear}")]
        public async Task<ActionResult> DeleteLeague(int leagueYear)
        {
            var leagueEntity = await _repo.GetLeagueAsync(leagueYear);
            if (leagueEntity == null)
            {
                return NotFound();
            }

            _repo.DeleteLeague(leagueEntity);
            await _repo.SaveChangesAsync();

            return NoContent();
        }
    }
}
