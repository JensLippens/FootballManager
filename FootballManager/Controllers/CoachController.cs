using AutoMapper;
using FootballManager.Entities;
using FootballManager.Models;
using FootballManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FootballManager.Controllers
{
    [ApiController]
    [Route("api/coaches")]
    public class CoachesController : ControllerBase
    {
        private readonly ILogger<CoachesController> _logger;
        private readonly IFootballManagerRepository _repo;
        private readonly IMapper _mapper;

        public CoachesController(ILogger<CoachesController> logger, IFootballManagerRepository repo, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoachDto>>> GetAllCoaches()
        {
            var coachEntities = await _repo.GetAllCoachesAsync();

            return Ok(_mapper.Map<IEnumerable<CoachDto>>(coachEntities));
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<CoachDto>> GetCoachFromTeam(int teamId)
        {
            if (!await _repo.TeamIdExistsAsync(teamId))
            {
                _logger.LogInformation($"Team with id {teamId} does not exists");
                return NotFound();
            }
            var coachEntity = await _repo.GetCoachFromTeamAsync(teamId);
            return Ok(_mapper.Map<CoachDto>(coachEntity));
        }

        [HttpGet("coach/{coachId}", Name = "GetCoach")]
        public async Task<ActionResult<CoachDto>> GetCoach(int coachId)
        {
            var coachEntity = await _repo.GetCoachAsync(coachId);
            if (coachEntity == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CoachDto>(coachEntity));
        }

        [HttpPost("team/{teamId}")]
        public async Task<ActionResult<CoachDto>> NewCoach(
            int teamId,
            CoachForCreationDto coach)
        {

            if (!await _repo.TeamIdExistsAsync(teamId))
            {
                return NotFound();
            }

            var coachToAdd = _mapper.Map<Entities.Coach>(coach);

            await _repo.AddCoachAsyncWithTeam(teamId, coachToAdd);
            await _repo.SaveChangesAsync();

            var coachToReturn = _mapper.Map<Models.CoachDto>(coachToAdd);

            return CreatedAtRoute("GetCoach",
                new
                {
                    playerId = coachToReturn.Id
                },
                coachToReturn);
        }

        [HttpPut("coach/{coachId}")]
        public async Task<ActionResult> UpdateCoachFull(
            int coachId,
            CoachForUpdateDto coach)
        {
            var coachEntity = await _repo.GetCoachAsync(coachId);
            if (coachEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(coach, coachEntity);

            await _repo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("coach/{coachId}")]
        public async Task<ActionResult> UpdateCoachPartial(
            int coachId,
            JsonPatchDocument<CoachForUpdateDto> patchDocument)
        {
            var coachEntity = await _repo.GetCoachAsync(coachId);
            if (coachEntity == null)
            {
                return NotFound();
            }

            var coachToPatch = _mapper.Map<CoachForUpdateDto>(coachEntity);

            patchDocument.ApplyTo(coachToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(coachToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(coachToPatch, coachEntity);

            await _repo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("coach/{coachId}")]
        public async Task<ActionResult> DeleteCoach(int coachId)
        {
            var coachEntity = await _repo.GetCoachAsync(coachId);
            if (coachEntity == null)
            {
                return NotFound();
            }

            _repo.DeleteCoach(coachEntity);
            await _repo.SaveChangesAsync();

            return NoContent();
        }

    }
}
