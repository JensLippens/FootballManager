using AutoMapper;
using FootballManager.Models;
using FootballManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ILogger<TeamsController> _logger;
        private readonly IFootballManagerRepository _repo;
        private readonly IMapper _mapper;

        public TeamsController(ILogger<TeamsController> logger, IFootballManagerRepository repo, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamWithoutGamesOrPlayersDto>>> GetAllTeams()
        {
            var teamEntities = await _repo.GetAllTeamsAsync();

            return Ok(_mapper.Map<IEnumerable<TeamWithoutGamesOrPlayersDto>>(teamEntities));
        }

        [HttpGet("league/{leagueYear}")]
        public async Task<ActionResult<IEnumerable<TeamWithoutGamesOrPlayersDto>>> GetAllTeamsForLeague(int leagueYear)
        {
            if (!await _repo.LeagueYearExistsAsync(leagueYear))
            {
                return BadRequest($"League {leagueYear} does not yet exist");
            }
            var teamEntities = await _repo.GetTeamsForLeagueAsync(leagueYear);

            return Ok(_mapper.Map<IEnumerable<TeamWithoutGamesOrPlayersDto>>(teamEntities));
        }

        [HttpGet("team/{teamId}", Name = "GetTeam")]
        public async Task<ActionResult<TeamDto>> GetTeam(int teamId)
        {
            var teamEntity = await _repo.GetTeamAsync(teamId);
            if (teamEntity == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TeamDto>(teamEntity));
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> AddTeam(TeamForCreationDto team)
        {          
            if (await _repo.TeamNameExistsAsync(team.Name))
            {
                return BadRequest($"A team with name {team.Name} already exists.");
            }
            var teamToAdd = _mapper.Map<Entities.Team>(team);

            await _repo.AddTeamAsync(teamToAdd);
            await _repo.SaveChangesAsync();

            var teamToReturn = _mapper.Map<Models.TeamDto>(teamToAdd);

            return CreatedAtRoute("GetTeam",
                new
                {
                    teamId = teamToReturn.Id
                },
                teamToReturn);
        }


        [HttpPatch("team/{teamId}/league/{leagueYear}")]
        public async Task<ActionResult> UpdateTeam(
            int teamId,
            int leagueYear,
            JsonPatchDocument<TeamForUpdateDto> patchDocument)
        {
            var teamEntity = await _repo.GetTeamAsync(teamId);
            if (teamEntity == null)
            {
                return NotFound();
            }

            if (teamEntity.Leagues.Any(l => l.Year == leagueYear))
            {
                return BadRequest($"Team {teamEntity.Name} is already part of league {leagueYear}");
            }
            var teamToPatch = _mapper.Map<TeamForUpdateDto>(teamEntity);

            patchDocument.ApplyTo(teamToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(teamToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(teamToPatch, teamEntity);

            await _repo.AddTeamToLeagueAsync(teamEntity, leagueYear);             
            await _repo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("team/{teamId}/league/{leagueYear}")]
        public async Task<ActionResult> RemoveTeamFromLeague(int teamId, int leagueYear)
        {
            var teamEntity = await _repo.GetTeamAsync(teamId);
            if (teamEntity == null)
            {
                return NotFound();
            }

            if (!teamEntity.Leagues.Any(l => l.Year == leagueYear))
            {
                return BadRequest($"Team {teamEntity.Name} is not part of league with year {leagueYear}");
            }

            await _repo.RemoveTeamFromLeagueAsync(teamEntity, leagueYear);
            await _repo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{teamId}")]
        public async Task<ActionResult> DeleteTeam(int teamId)
        {
            var teamEntity = await _repo.GetTeamAsync(teamId);
            if (teamEntity == null)
            {
                return NotFound();
            }

            _repo.DeleteTeam(teamEntity);
            await _repo.SaveChangesAsync();

            return NoContent();
        }
    }
}
