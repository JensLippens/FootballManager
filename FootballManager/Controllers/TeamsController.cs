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

        [HttpGet("{teamId}", Name = "GetTeam")]
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
        public async Task<ActionResult<TeamDto>> NewTeam(TeamForCreationDto team)
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

        [HttpPatch("{teamId}")]
        public async Task<ActionResult> UpdateTeam(
            int teamId,
            JsonPatchDocument<TeamForUpdateDto> patchDocument)
        {
            var teamEntity = await _repo.GetTeamAsync(teamId);
            if (teamEntity == null)
            {
                return NotFound();
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
