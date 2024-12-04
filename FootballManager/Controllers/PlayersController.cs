using FootballManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using FootballManager.Services;
using AutoMapper;

namespace FootballManager.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {   
        private readonly ILogger<PlayersController> _logger;
        private readonly IFootballManagerRepository _repo;
        private readonly IMapper _mapper;

        public PlayersController(ILogger<PlayersController> logger, IFootballManagerRepository repo, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAllPlayers()
        {
            var playerEntities = await _repo.GetAllPlayersAsync();

            return Ok(_mapper.Map<IEnumerable<PlayerDto>>(playerEntities));
        }
        
        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayersFromTeam(int teamId)
        {           
            if (!await _repo.TeamIdExistsAsync(teamId))
            {
                _logger.LogInformation($"Team with id {teamId} does not exists");
                return NotFound();
            }
            var playerEntities = await _repo.GetPlayersFromTeamAsync(teamId);
            return Ok(_mapper.Map<IEnumerable<PlayerDto>>(playerEntities));
        }

        [HttpGet("player/{playerId}", Name = "GetPlayer")]
        public async Task<ActionResult<PlayerDto>> GetPlayer(int playerId)
        {
            var playerEntity = await _repo.GetPlayerAsync(playerId);
            if (playerEntity == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PlayerDto>(playerEntity));
        }

        [HttpPost("team/{teamId}")]
        public async Task<ActionResult<PlayerDto>> NewPlayer(
            int teamId,
            PlayerForCreationDto player) 
        {

            if (!await _repo.TeamIdExistsAsync(teamId))
            {
                return NotFound();
            }

            var playerToAdd = _mapper.Map<Entities.Player>(player);

            await _repo.AddPlayerAsyncWithTeam(teamId, playerToAdd);
            await _repo.SaveChangesAsync();

            var playerToReturn = _mapper.Map<Models.PlayerDto>(playerToAdd);

            return CreatedAtRoute("GetPlayer",
                new
                {
                    playerId = playerToReturn.Id
                },
                playerToReturn);
        }
        
        [HttpPut("player/{playerId}")]
        public async Task<ActionResult> UpdatePlayerFull(
            int playerId,
            PlayerForUpdateDto player) 
        {
            var playerEntity = await _repo.GetPlayerAsync(playerId);
            if (playerEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(player, playerEntity);

            await _repo.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPatch("player/{playerId}")]
        public async Task<ActionResult> UpdatePlayerPartial(
            int playerId,
            JsonPatchDocument<PlayerForUpdateDto> patchDocument)
        {
            var playerEntity = await _repo.GetPlayerAsync(playerId);
            if (playerEntity == null)
            {
                return NotFound();
            }

            var playerToPatch = _mapper.Map<PlayerForUpdateDto>(playerEntity);

            patchDocument.ApplyTo(playerToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(playerToPatch))
            {
                return BadRequest(ModelState);
            }

           _mapper.Map(playerToPatch, playerEntity);

            await _repo.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("player/{playerId}")]
        public async Task<ActionResult> DeletePlayer(int playerId)
        {
            var playerEntity = await _repo.GetPlayerAsync(playerId);
            if (playerEntity == null)
            {
                return NotFound();
            }

            _repo.DeletePlayer(playerEntity);
            await _repo.SaveChangesAsync();

            return NoContent();
        }
        
    }
}
