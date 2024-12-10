using FootballManager.Entities;
using FootballManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace FootballManager.Services
{
    public interface IFootballManagerRepository
    {
        Task<bool> SaveChangesAsync();

        //PLAYERS
        Task<(IEnumerable<Player>, PaginationMetadata)> GetAllPlayersAsync(
                string? searchQuery, Position? position, int pageNumber, int pageSize);
        Task<Player?> GetPlayerAsync(int playerId);
        Task<IEnumerable<Player>> GetPlayersFromTeamAsync(int teamId);
        Task AddPlayerAsyncWithTeam(int teamId, Player player);
        Task<bool> ShirtNumberAlreadyTaken(int teamId, int shirtNumber);
        Task RemovePlayerFromTeamAsync(Player player, int? teamId);
        void DeletePlayer(Player player);

        //COACHES
        Task<IEnumerable<Coach>> GetAllCoachesAsync();
        Task<IEnumerable<Coach>> GetAllCoachesAsync(string? searchQuery);
        Task<Coach?> GetCoachAsync(int coachId);
        Task<Coach?> GetCoachFromTeamAsync(int teamId);
        Task AddCoachAsyncWithTeam(int teamId, Coach coach);
        Task RemoveCoachFromTeamAsync(Coach coach, int? teamId);
        void DeleteCoach(Coach coach);

        //TEAMS
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<IEnumerable<Team>> GetTeamsForLeagueAsync(int leagueYear);
        Task<Team?> GetTeamAsync(int? teamId);
        Task<bool> TeamIdExistsAsync(int teamId);
        Task<bool> TeamNameExistsAsync(string name);
        Task<bool> TeamsArePartOfLeagueAsync(int homeTeamId, int awayTeamId, int leagueYear);
        Task AddTeamAsync(Team team);
        Task AddTeamToLeagueAsync(Team team, int leagueYear);
        Task RemoveTeamFromLeagueAsync(Team team, int leagueYear);
        void DeleteTeam(Team team);

        //LEAGUES
        Task<IEnumerable<League>> GetAllLeaguesAsync();
        Task<League?> GetLeagueAsync(int leagueYear, bool includeGames = false, bool includeTeams = false);
        Task<bool> LeagueYearExistsAsync(int leagueYear);
        Task AddLeagueAsync(League league);
        void DeleteLeague(League league);

        //GAMES

        Task<IEnumerable<Game>> GetAllGamesFromSpecificLeagueAsync(int leagueYear);
        Task<(IEnumerable<Game>, PaginationMetadata)> GetGamesFromSpecificLeagueAsync(int leagueYear, int? teamId, int pageNumber, int pageSize);
        Task<Game?> GetGameWithHomeTeamAndAwayTeamAsync(int homeTeamId, int awayTeamId, int leagueYear);
        Task<Game?> GetGameAsync(int gameId);
        Task AddGameAsync(Game game);

        //STANDINGS
        Task<IEnumerable<Standing>> GetCurrentLeagueStandingsAsync();
        Task<IEnumerable<Standing>> GetLeagueYearStandingsAsync(int leagueYear);
        Task<Standing?> GetStandingForTeamInLeague(int teamId, int leagueYear);
    }
}
