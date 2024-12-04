﻿using FootballManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public interface IFootballManagerRepository
    {
        Task<bool> SaveChangesAsync();

        //PLAYERS
        Task<IEnumerable<Player>> GetAllPlayersAsync();
        Task<Player?> GetPlayerAsync(int playerId);
        Task<IEnumerable<Player>> GetPlayersFromTeamAsync(int teamId);
        Task AddPlayerAsyncWithTeam(int teamId, Player player);
        //void AddPlayerAsyncWithoutTeam(Player player);
        void DeletePlayer(Player player);

        //COACHES
        Task<IEnumerable<Coach>> GetAllCoachesAsync();
        Task<Coach?> GetCoachAsync(int coachId);
        Task<Coach?> GetCoachFromTeamAsync(int teamId);
        Task AddCoachAsyncWithTeam(int teamId, Coach coach);
        //void AddCoachrAsyncWithoutTeam(Coach player);
        void DeleteCoach(Coach coach);

        //TEAMS
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamAsync(int? teamId);
        Task<bool> TeamIdExistsAsync(int teamId);
        Task<bool> TeamNameExistsAsync(string name);
        Task AddTeamAsync(Team team);
        void DeleteTeam(Team team);

        //LEAGUES
        Task<IEnumerable<League>> GetAllLeaguesAsync();
        Task<League?> GetLeagueAsync(int leagueYear, bool includeGames = false, bool includeTeams = false);
        Task AddLeagueAsync(League league);

        //GAMES
        Task<IEnumerable<Game>> GetGamesFromSpecificLeagueAsync(int leagueYear);
        Task<IEnumerable<Game>> GetGamesFromTeamInSpecificLeagueAsync(int leagueYear, int teamId);
        Task<Game?> GetGameWithHomeTeamAndAwayTeamAsync(int homeTeamId, int awayTeamId, int leagueYear);
        Task<Game?> GetGameAsync(int gameId);
        Task AddGameAsync(Game game);

        //STANDINGS
        Task<IEnumerable<Standing>> GetCurrentLeagueStandingsAsync();
        Task<IEnumerable<Standing>> GetLeagueYearStandingsAsync(int leagueYear);
        Task<Standing?> GetStandingForTeamInLeague(int teamId, int leagueYear);
    }
}