﻿using AutoMapper;
using FootballManager.Entities;
using FootballManager.Models;
using FootballManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<CoachDto>>> GetAllCoaches(
            string? searchQuery)
        {
            var coachEntities = await _repo.GetAllCoachesAsync(searchQuery);

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
            var team = await _repo.GetTeamAsync(teamId);

            if (team == null)
            {
                return NotFound();
            }

            var coachToAdd = _mapper.Map<Entities.Coach>(coach);

            if (team.Coach != null)
            {
                team.Coach.TeamId = null;
            }

            await _repo.AddCoachAsyncWithTeam(team.Id, coachToAdd);
            await _repo.SaveChangesAsync();

            var coachToReturn = _mapper.Map<Models.CoachDto>(coachToAdd);

            return CreatedAtRoute("GetCoach",
                new
                {
                    coachId = coachToReturn.Id
                },
                coachToReturn);
        }

        [HttpPut("coach/{coachId}")]
        public async Task<ActionResult> UpdateCoachFull(
            int coachId,
            CoachForUpdateDto coach)
        {
            if (coach.TeamId != null)
            {
                if (!await _repo.TeamIdExistsAsync((int)coach.TeamId))
                {
                    _logger.LogInformation($"Team with id {coach.TeamId} does not exists");
                    return NotFound();
                }
            }

            var coachEntity = await _repo.GetCoachAsync(coachId);
            if (coachEntity == null)
            {
                return NotFound();
            }

            
            if (coach.TeamId != null && coachEntity.TeamId != coach.TeamId)
            {
                if (coachEntity.TeamId != null)
                {
                    await _repo.RemoveCoachFromTeamAsync(coachEntity, coachEntity.TeamId);
                    await _repo.AddCoachAsyncWithTeam((int)coach.TeamId, coachEntity);
                }
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
            
            if (coachToPatch.TeamId != null)
            {
                var currentCoach = await _repo.GetCoachFromTeamAsync((int)coachToPatch.TeamId);
                if (currentCoach != null)
                {
                    currentCoach.TeamId = null;
                }
            }
            _mapper.Map(coachToPatch, coachEntity);

            await _repo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("team/{teamId}/coach/{coachId}")]
        public async Task<ActionResult> RemoveCoachFromTeam(int teamId, int coachId)
        {
            var coachEntity = await _repo.GetCoachAsync(coachId);
            if (coachEntity == null)
            {
                return NotFound();
            }

            if (coachEntity.TeamId != teamId)
            {
                return BadRequest($"Coach {coachEntity.FirstName} {coachEntity.LastName} is not part of the team with Id {teamId}");
            }

            await _repo.RemoveCoachFromTeamAsync(coachEntity, teamId);
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
