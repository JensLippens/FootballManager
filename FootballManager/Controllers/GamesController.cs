using AutoMapper;
using FootballManager.Entities;
using FootballManager.Models;
using FootballManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FootballManager.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ILogger<GamesController> _logger;
        private readonly IFootballManagerRepository _repo;
        private readonly IMapper _mapper;
        const int maximumPageSize = 15;

        public GamesController(ILogger<GamesController> logger, IFootballManagerRepository repo, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("table/league/{leagueYear}")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGamesFromSpecificLeague(int leagueYear)
        {
            var leagueEntity = await _repo.GetLeagueAsync(leagueYear);
            if (leagueEntity == null)
            {
                return NotFound();
            }
           
            var gamesEntities = await _repo.GetAllGamesFromSpecificLeagueAsync(leagueYear);

            return Ok(_mapper.Map<IEnumerable<GameDto>>(gamesEntities));
        }


        [HttpGet("league/{leagueYear}")]

        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesFromSpecificLeague(int leagueYear,
            [FromQuery] int? teamId, int pageNumber = 1, int pageSize = 8)
        {
            var leagueEntity = await _repo.GetLeagueAsync(leagueYear);
            if (leagueEntity == null)
            {
                return NotFound();
            }
            if (pageSize > maximumPageSize)
            {
                pageSize = maximumPageSize;
            }
            var (gamesEntities, paginationMetadata) = await _repo.GetGamesFromSpecificLeagueAsync(leagueYear, teamId, pageNumber, pageSize);

            Response.Headers.Append("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<GameDto>>(gamesEntities));
        }

        /*
        [HttpGet("league/{leagueYear}/team/{teamId}")]

        public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesFromTeamInSpecificLeague(int leagueYear, int teamId)
        {
            var leagueEntity = await _repo.GetLeagueAsync(leagueYear);
            if (leagueEntity == null || !await _repo.TeamIdExistsAsync(teamId))
            {
                return NotFound();
            }

            var gamesEntities = await _repo.GetGamesFromTeamInSpecificLeagueAsync(leagueYear, teamId);
            return Ok(_mapper.Map<IEnumerable<GameDto>>(gamesEntities));
        }
        */

        [HttpGet("game/{gameId}", Name = "GetGame")]

        public async Task<ActionResult<GameDto>> GetGame(int gameId)
        {
            var gameEntity = await _repo.GetGameAsync(gameId);
            if (gameEntity == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GameDto>(gameEntity));
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> NewGame(GameForCreationDto game)
        {
            if (!await _repo.LeagueYearExistsAsync(game.LeagueYear))
            {
                return BadRequest($"League {game.LeagueYear} does not yet exist");
            }

            var gameEntity = await _repo.GetGameWithHomeTeamAndAwayTeamAsync(game.HomeTeamId, game.AwayTeamId, game.LeagueYear);
            
            if (gameEntity != null)
            {
                return BadRequest($"A Game between home team {gameEntity?.HomeTeam?.Name} and away team {gameEntity?.AwayTeam?.Name} already exists for league {gameEntity?.LeagueYear} on matchday {gameEntity?.MatchDay}");
            }

            if (!await _repo.TeamsArePartOfLeagueAsync(game.HomeTeamId, game.AwayTeamId, game.LeagueYear))
            {
                return BadRequest($"Teams with IDs {game.HomeTeamId} and/or {game.AwayTeamId} are not part of league {game.LeagueYear}");
            }


            var gameToAdd = _mapper.Map<Entities.Game>(game);

            await _repo.AddGameAsync(gameToAdd);
            await _repo.SaveChangesAsync();

            if (game.HomeTeamScore != null && game.AwayTeamScore != null)
            {
                UpdateStandings(gameToAdd);
            }

            await _repo.SaveChangesAsync();

            var gameToReturn = _mapper.Map<Models.GameDto>(gameToAdd);

            return CreatedAtRoute("GetPlayer",
                new
                {
                    gameId = gameToReturn.Id
                },
                gameToReturn);
        }

        [HttpPatch("{gameId}")]
        public async Task<ActionResult> UpdateGameScore(
            int gameId,
            JsonPatchDocument<GameForUpdateDto> patchDocument)
        {
            var gameEntity = await _repo.GetGameAsync(gameId);
            if (gameEntity == null)
            {
                return NotFound();
            }

            var gameToPatch = _mapper.Map<GameForUpdateDto>(gameEntity);

            patchDocument.ApplyTo(gameToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(gameToPatch))
            {
                return BadRequest(ModelState);
            }

            // Only allow updates that include scores.
            // If Game already had scores befpre (ex. in case game had to be replayed),
            // update the standings to remove these statistics before applying the new ones.

            if (gameEntity.HomeTeamScore != null && gameEntity.AwayTeamScore != null)
            {
                RemovePreviousStandings(gameEntity);
            }
            
            _mapper.Map(gameToPatch, gameEntity);

            UpdateStandings(gameEntity);

            await _repo.SaveChangesAsync();

            return NoContent();
        }

        private async void RemovePreviousStandings(Game game)
        {
            var homeTeamStanding = await _repo.GetStandingForTeamInLeague(game.HomeTeamId, game.LeagueYear);
            var awayTeamStanding = await _repo.GetStandingForTeamInLeague(game.AwayTeamId, game.LeagueYear);

            if (homeTeamStanding != null && awayTeamStanding != null)
            {
                homeTeamStanding.GamesPlayed--;
                awayTeamStanding.GamesPlayed--;

                homeTeamStanding.GoalsFor -= game.HomeTeamScore.GetValueOrDefault();
                homeTeamStanding.GoalsAgainst -= game.AwayTeamScore.GetValueOrDefault();

                awayTeamStanding.GoalsFor -= game.AwayTeamScore.GetValueOrDefault();
                awayTeamStanding.GoalsAgainst -= game.HomeTeamScore.GetValueOrDefault();

                if (game.HomeTeamScore > game.AwayTeamScore)
                {
                    homeTeamStanding.Wins--;
                    awayTeamStanding.Losses--;
                }
                else if (game.HomeTeamScore < game.AwayTeamScore)
                {
                    awayTeamStanding.Wins--;
                    homeTeamStanding.Losses--;
                }
                else
                {
                    homeTeamStanding.Draws--;
                    awayTeamStanding.Draws--;
                }
                await _repo.SaveChangesAsync();
            }
        }

        private async void UpdateStandings(Game game)
        {
            var homeTeamStanding = await _repo.GetStandingForTeamInLeague(game.HomeTeamId, game.LeagueYear);
            var awayTeamStanding = await _repo.GetStandingForTeamInLeague(game.AwayTeamId, game.LeagueYear);

            if (homeTeamStanding != null && awayTeamStanding != null)
            {
                homeTeamStanding.GamesPlayed++;
                awayTeamStanding.GamesPlayed++;

                homeTeamStanding.GoalsFor += game.HomeTeamScore.GetValueOrDefault();
                homeTeamStanding.GoalsAgainst += game.AwayTeamScore.GetValueOrDefault();

                awayTeamStanding.GoalsFor += game.AwayTeamScore.GetValueOrDefault();
                awayTeamStanding.GoalsAgainst += game.HomeTeamScore.GetValueOrDefault();

                if (game.HomeTeamScore > game.AwayTeamScore)
                {
                    homeTeamStanding.Wins++;
                    awayTeamStanding.Losses++;
                }
                else if (game.HomeTeamScore < game.AwayTeamScore)
                {
                    awayTeamStanding.Wins++;
                    homeTeamStanding.Losses++;
                }
                else
                {
                    homeTeamStanding.Draws++;
                    awayTeamStanding.Draws++;
                }
                await _repo.SaveChangesAsync();
            }
        }
    }
}