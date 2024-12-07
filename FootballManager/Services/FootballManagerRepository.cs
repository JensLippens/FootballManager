using FootballManager.DbContexts;
using FootballManager.Entities;
using FootballManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class FootballManagerRepository : IFootballManagerRepository
    {
        private readonly FootballManagerContext _context;

        public FootballManagerRepository(FootballManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        /// <summary>
        /// PLAYERS
        /// </summary>
        /// 

        /*
        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _context.Players
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .ToListAsync();
        }
        */
        public async Task<(IEnumerable<Player>, PaginationMetadata)> GetAllPlayersAsync(string? searchQuery, Position? position, int pageNumber, int pageSize)
        {
            var playersCollection = _context.Players as IQueryable<Player>;

            if (position != null)
            {
                playersCollection = playersCollection.Where(p => p.Position == position);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                playersCollection = playersCollection.Where(p => p.FirstName.ToLower().Contains(searchQuery.ToLower()) ||
                                                                 p.LastName.ToLower().Contains(searchQuery.ToLower()));

            }

            var totalItemCount = await playersCollection.CountAsync();

            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn=  await playersCollection               
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<Player?> GetPlayerAsync(int playerId)
        {
            return await _context.Players
                .Where(p => p.Id == playerId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Player>> GetPlayersFromTeamAsync(int teamId)
        {
            return await _context.Players
                .Where(p => p.TeamId == teamId)
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task AddPlayerAsyncWithTeam(int teamId, Player player)
        {
            var team = await GetTeamAsync(teamId);
            if (team != null)
            {
                team.Players.Add(player);
            }
        }

        public async Task<bool> ShirtNumberAlreadyTaken(int teamId, int shirtNumber)
        {
            return await _context.Players
                .Where(p => p.TeamId == teamId)
                .AnyAsync(p => p.ShirtNumber == shirtNumber);

        }
        /*
        public void AddPlayerAsyncWithoutTeam(Player player)
        {
            _context.Players.Add(player);
        }
        */

        public async Task RemovePlayerFromTeamAsync(Player player, int? teamId)
        {
            var team = await GetTeamAsync(teamId);
            if (team != null)
            {
                team.Players.Remove(player);
            }
        }

        public void DeletePlayer(Player player)
        {
            _context.Players.Remove(player);
        }


        /// <summary>
        /// COACHES
        /// </summary>
        /// 

        public async Task<IEnumerable<Coach>> GetAllCoachesAsync()
        {
            return await _context.Coaches
                .Include(c => c.Team)
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coach>> GetAllCoachesAsync(string? searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return await GetAllCoachesAsync();
            }

            searchQuery = searchQuery.Trim();

            return await _context.Coaches
                .Include(c => c.Team)
                .Where(c => c.FirstName.ToLower().Contains(searchQuery.ToLower()) || c.LastName.ToLower().Contains(searchQuery.ToLower()))
                .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                .ToListAsync();
        }
        public async Task<Coach?> GetCoachAsync(int coachId)
        {
            return await _context.Coaches                
                .Include(c => c.Team)
                .Where(c => c.Id == coachId)
                .FirstOrDefaultAsync();
        }

        public async Task<Coach?> GetCoachFromTeamAsync(int teamId)
        {
            return await _context.Coaches
                .Include(c => c.Team)
                .Where(c => c.TeamId == teamId)
                .FirstOrDefaultAsync();
        }

        public async Task AddCoachAsyncWithTeam(int teamId, Coach coach)
        {
            var team = await GetTeamAsync(teamId);
            if (team != null)
            {
                team.Coach = coach;
                coach.TeamId = teamId;
            }
        }
        /*
        public void AddCoachAsyncWithoutTeam(Coach coach)
        {
            _context.Coaches.Add(coach);
        }
        */
        public async Task RemoveCoachFromTeamAsync(Coach coach, int? teamId)
        {
            var team = await GetTeamAsync(teamId);
            if (team != null)
            {
                team.Coach = null;
            }
        }

        public void DeleteCoach(Coach coach)
        {
            _context.Coaches.Remove(coach);
        }

        /// <summary>
        /// TEAMS
        /// </summary>
        /// 

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Team?> GetTeamAsync(int? teamId)
        {
            return await _context.Teams
                .Where(t => t.Id == teamId)
                .Include(t => t.Leagues)
                .Include(t => t.Games)
                .Include(t => t.Players)
                .Include(t => t.Coach)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsForLeagueAsync(int leagueYear)
        {
           return await _context.Teams
                .Where(t => t.Leagues.Any(l => l.Year == leagueYear))
                .ToListAsync();
        }

        public async Task<bool> TeamIdExistsAsync(int teamId)
        {
            return await _context.Teams.AnyAsync(t => t.Id == teamId);
        }
        public async Task<bool> TeamNameExistsAsync(string name)
        {
            return await _context.Teams.AnyAsync(t => t.Name == name);
        }
        public async Task<bool> TeamsArePartOfLeagueAsync(int homeTeamId, int awayTeamId,int leagueYear)
        {
            var league = await GetLeagueAsync(leagueYear, true, true);
            if (league != null)
            {
                return league.Teams.Any(t => t.Id == homeTeamId) && 
                       league.Teams.Any(t => t.Id == awayTeamId);
            }
            return false;
        }

        public async Task AddTeamAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
        }

        public async Task AddTeamToLeagueAsync(Team team, int leagueYear)
        {
            var league = await GetLeagueAsync(leagueYear);
            if (league != null)
            {
                league.Teams.Add(team);
                team.Leagues.Add(league);
                await CreateNewStandingAsync(new Standing(team.Id, leagueYear));
            }
        }

        public async Task RemoveTeamFromLeagueAsync(Team team, int leagueYear)
        {
            var league = await GetLeagueAsync(leagueYear);
            if (league != null)
            {
                league.Teams.Remove(team);
                team.Leagues.Remove(league);
            }
        }

        public void DeleteTeam(Team team)
        {
            _context.Teams.Remove(team);
        }

        /// <summary>
        /// LEAGUES
        /// </summary>
        /// 
        public async Task<IEnumerable<League>> GetAllLeaguesAsync()
        {
            return await _context.Leagues
                .OrderBy(l => l.Year)
                .ToListAsync();
        }

        public async Task<League?> GetLeagueAsync(int leagueYear, bool includeGames = false, bool includeTeams = false)
        {
            IQueryable<League> query = _context.Leagues;
            
            if (includeGames)
            {
                query = query
                    .Include(l => l.Games).ThenInclude(g => g.HomeTeam)
                    .Include(l => l.Games).ThenInclude(g => g.AwayTeam);
            }
            if (includeTeams)
            {
                query = query.Include(l => l.Teams).ThenInclude(t => t.Coach);
            }
            return await query
                .Where(l => l.Year == leagueYear)
                .FirstOrDefaultAsync();            
        }

        public async Task<bool> LeagueYearExistsAsync(int leagueYear)
        {
            return await _context.Leagues.AnyAsync(l => l.Year == leagueYear);
        }

        public async Task AddLeagueAsync(League league)
        {
           var leagueTeams = await GetTeamsForLeagueAsync(league.Year - 1);
           if (leagueTeams != null)
           {
                league.Teams = leagueTeams.ToList();
                foreach (var team in leagueTeams)
                {
                    await CreateNewStandingAsync(new Standing(team.Id, league.Year));
                }
           }
           await _context.Leagues.AddAsync(league);
        }

        public void DeleteLeague(League league)
        {
            _context.Leagues.Remove(league);
        }
        /// <summary>
        /// GAMES
        /// </summary>
        ///
        /*
        public async Task<IEnumerable<Game>> GetGamesFromSpecificLeagueAsync(int leagueYear, int pageNumber, int pageSize)
        {
            return await _context.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Where(g => g.LeagueYear == leagueYear)
                .OrderBy(g => g.MatchDay)
                .ToListAsync();
        }
        */

        public async Task<(IEnumerable<Game>, PaginationMetadata)> GetGamesFromSpecificLeagueAsync(int leagueYear, string? teamName, int pageNumber, int pageSize)
        {
            var gamesCollection = _context.Games as IQueryable<Game>;

            gamesCollection = gamesCollection
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam) 
                .Where(g => g.LeagueYear == leagueYear);

            if (!string.IsNullOrEmpty(teamName))
            {
                teamName = teamName.Trim();
                gamesCollection = gamesCollection.Where(g => g.HomeTeam!.Name == teamName || g.AwayTeam!.Name == teamName);
            }            
            
            var totalItemCount = await gamesCollection.CountAsync();

            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await gamesCollection
                .OrderBy(g => g.MatchDay)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }
        /*
        public async Task<IEnumerable<Game>> GetGamesFromTeamInSpecificLeagueAsync(int leagueYear, int teamId)
        {
            return await _context.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Where(g => g.LeagueYear == leagueYear && (g.HomeTeamId == teamId || g.AwayTeamId == teamId))
                .OrderBy(g => g.MatchDay)
                .ToListAsync();
        }
        */

        public async Task<Game?> GetGameWithHomeTeamAndAwayTeamAsync(int homeTeamId, int awayTeamId, int leagueYear)
        {
            return await _context.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Where(g => g.HomeTeamId == homeTeamId && g.AwayTeamId == awayTeamId && g.LeagueYear == leagueYear)
                .FirstOrDefaultAsync();
        }
        public async Task<Game?> GetGameAsync(int gameId)
        {
            return await _context.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .Where(g => g.Id == gameId)
                .FirstOrDefaultAsync();
        }

        public async Task AddGameAsync(Game game)
        {
            var league = await GetLeagueAsync(game.LeagueYear);
            if (league != null)
            {
                league.Games.Add(game);
            }
        }

        /// <summary>
        /// STANDINGS
        /// </summary>
        ///

        public async Task<IEnumerable<Standing>> GetCurrentLeagueStandingsAsync()
        {
            var mostRecentYear = await _context.Leagues
                .OrderByDescending(l => l.Year)
                .Select(l => l.Year)
                .FirstOrDefaultAsync();

            return await _context.Standings
                .Where(s => s.LeagueYear == mostRecentYear)
                .Include(s => s.Team)
                .ToListAsync();
        }
        public async Task<IEnumerable<Standing>> GetLeagueYearStandingsAsync(int leagueYear)
        {
            return await _context.Standings
                .Where(s => s.LeagueYear == leagueYear)
                .Include(s => s.Team)
                .ToListAsync();
        }

        public async Task<Standing?> GetStandingForTeamInLeague(int teamId, int leagueYear)
        {
            return await _context.Standings
                .Where(s => s.TeamId == teamId && s.LeagueYear == leagueYear)
                .FirstOrDefaultAsync();
        }

        public async Task CreateNewStandingAsync(Standing standing)
        {
            await _context.Standings.AddAsync(standing);
        }
    }
}
