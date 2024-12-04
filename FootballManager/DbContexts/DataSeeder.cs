using Bogus.DataSets;
using Bogus;
using FootballManager.Entities;
using FootballManager.Models;
using SQLitePCL;

namespace FootballManager.DbContexts
{
    public class DataSeeder
    {
        private readonly FootballManagerContext _context;
        public DataSeeder(FootballManagerContext context)
        {
            _context = context;
        }

        Random random = new Random();
        //string[] firstNames = new string[] { "Noah", "Arthur", "Liam", "Adam", "Louis", "Jules", "Lucas", "Gabriel", "Victor", "Matteo", "Ayden", "Ibrahim", "Noé", "Lucien", "Gaston", "Basile", "Maurice", "Achille", "Oscar", "Henri", "Leon", "Marius", "Elias", "Owen", "Ali", "Theo", "Lucien", "Raphaël", "Samuel", "Rayan", "David", "Clément", "Marcel", "Basile", "Sacha", "Gaston", "Benoit", "Cedric", "Axel", "Logan", "Mick", "Mike", "Maxime", "Martin", "Sylvain", "Rayan", "Bastien", "Zane", "Frederic", "Gregoire", "Xavier", "Dimitri" };
        //string[] lastNames = new string[] { "Janssens", "Peeters", "Maes", "Lemmens", "Jacobs", "Mertens", "Wouters", "Rombouts", "Vermeulen", "Peters", "Verhoeven", "Vermeyen", "Huygens", "Bosmans", "De Smet", "Vermeulen", "Bogaerts", "De Clercq", "Coolen", "Vanderhaegen", "Geerts", "Vanschoonbeek", "Vandermeulen", "Verstraeten", "Vanhove", "Vervoort", "Vandenberghe", "De Vries", "Van den Berg", "De Cock", "Carlier", "Nivelle", "Devos", "Decock", "Gits", "Van Gorp", "Bourgeois", "De Vos", "Loos", "Schepers", "Bauwens", "De Jonge", "Vercammen", "Demeyer", "Creten", "Pauwels" };
        string[] belgianTeams = new string[] { "Anderlecht", "Antwerp", "Cercle Brugge", "Charleroi", "Club Brugge", "KAA Gent", "Kortrijk", "KRC Genk", "KV Mechelen", "OH Leuven", "Standard Luik", "STVV", "Union", "Westerlo", "Beerschot", "Dender", "Eupen", "RWDM" };

        public void SeedData()
        {
            if (_context.Leagues.Any())
            {
                return; // Exit if data already exists
            }

            var faker = new Faker();

            // Create Leagues
            var leagueCurrent = new League(2025);
            var leaguePrevious = new League(2024);
            _context.Leagues.Add(leagueCurrent);
            _context.Leagues.Add(leaguePrevious);
            _context.SaveChanges();  

            // Create Teams
            var teams = new List<Team>();
            for (int i = 0; i < belgianTeams.Length; i++)
            {
                teams.Add(new Team(belgianTeams[i]));
            }

            var league2025Teams = teams.Take(16).ToList();
            var league2024Teams = teams.Take(14).Concat(teams.Skip(16)).ToList();

            leagueCurrent.Teams.AddRange(league2025Teams);
            leaguePrevious.Teams.AddRange(league2024Teams);
            _context.SaveChanges();  

            // Create Players
            var players = new List<Player>();
            foreach (var team in teams)
            {
                for (int i = 0; i < 24; i++)  // 24 players per team: 3/7/7/7
                {
                    var position = i switch
                    {
                        0 or 1 or 2 => Position.Goalkeeper,  // First 3 players are Goalkeepers
                        >= 3 and <= 9 => Position.Defender,  // Next 7 players are Defenders
                        >= 10 and <= 16 => Position.Midfielder, // Next 7 players are Midfielders
                        _ => Position.Striker, // The rest are Strikers
                    };

                    var shirtNumber = i == 0 ? 1 : faker.Random.Number(2, 99); // Shirt number 1 for the first goalkeeper, else random number between 2-99

                    players.Add(new Player
                    {
                        FirstName = faker.Name.FirstName(Name.Gender.Male),
                        LastName = faker.Name.LastName(),
                        DateOfBirth = faker.Date.Past(20, DateTime.Now.AddYears(-18)), // Age between 18-38
                        IsLeftFooted = faker.Random.Number(1,99) > 80 ? true : false,
                        Position = position,
                        ShirtNumber = shirtNumber,
                        TeamId = team.Id
                    });
                }
            }

            _context.Players.AddRange(players);
            _context.SaveChanges();

            // Create Coaches
            var coaches = new List<Coach>();
            foreach (var team in teams)
            {
                var dateOfBirth = faker.Date.Past(30, DateTime.Now.AddYears(-35)); // Age between 35-65
                int age = DateTime.Now.Year - dateOfBirth.Year;

                var yearsOfExperience = age switch
                {
                    < 40 => faker.Random.Number(1, 5),
                    >= 40 and < 50 => faker.Random.Number(5, 10),
                    >= 50 and < 60 => faker.Random.Number(10, 15),
                    _ => faker.Random.Number(15, 20)
                };

                coaches.Add(new Coach
                {
                    FirstName = faker.Name.FirstName(Name.Gender.Male),
                    LastName = faker.Name.LastName(),
                    DateOfBirth = dateOfBirth,
                    YearsOfExperience = yearsOfExperience,
                    TeamId = team.Id
                });
            }

            _context.Coaches.AddRange(coaches);
            _context.SaveChanges(); 

            // Create Games and Standings
            var leagues = _context.Leagues.ToList();
            var teamsCurrent = leagueCurrent.Teams.ToList();
            var teamsPrevious = leaguePrevious.Teams.ToList();
            var totalMatchdays = (teamsCurrent.Count - 1) * 2;

            var league2025Games = GenerateRoundRobinSchedule(league2025Teams, 2025, 10, 30, new DateTime(2024,7,27));
            var league2024Games = GenerateRoundRobinSchedule(league2024Teams, 2024, 30, 30, new DateTime(2023,7,29));

            leagueCurrent.Games.AddRange(league2025Games);
            leaguePrevious.Games.AddRange(league2024Games);

            var standings2025 = league2025Teams.Select(t => new Standing { TeamId = t.Id, LeagueYear = 2025 }).ToList();
            var standings2024 = league2024Teams.Select(t => new Standing { TeamId = t.Id, LeagueYear = 2024 }).ToList();

            UpdateStandings(league2025Games, standings2025);
            UpdateStandings(league2024Games, standings2024);

            // Add leagues, games, and standings to the database
            _context.Standings.AddRange(standings2024.Concat(standings2025));
            _context.SaveChanges();

            /*
            _context.Standings.AddRange(standings2025);
            _context.SaveChanges();
            
            
            // Round-robin logic to generate the schedule
            var matchdays = new List<List<Game>>();

            for (int matchDay = 1; matchDay <= totalMatchdays; matchDay++)
            {
                var matchups = new List<Game>();

                // Generate home-away pairs for each matchday
                for (int i = 0; i < totalTeams / 2; i++)
                {
                    int homeTeamId = (matchDay + i) % (totalTeams);
                    int awayTeamId = (totalTeams - 1 - i + matchDay) % (totalTeams);

                    if (homeTeamId == awayTeamId) continue; // Skip invalid pairings

                    var homeTeam = teams[homeTeamId];
                    var awayTeam = teams[awayTeamId];

                    // Create a home-away game pair
                    matchups.Add(new Game
                    {
                        MatchDay = matchDay,
                        HomeTeamId = homeTeam.Id,
                        AwayTeamId = awayTeam.Id,
                        Date = new DateTime(2024,7,24).AddDays(matchDay*7),
                        LeagueId = leagueCurrent.Year,
                    });
                }

                // Add generated matchups for the matchday to the list of matchdays
                matchdays.Add(matchups);
            }

            // Now create the games and update standings
            foreach (var matchday in matchdays)
            {
                foreach (var game in matchday)
                {
                    // If it's one of the completed matchdays, assign random scores
                    if (game.MatchDay <= completedMatchdays)
                    {
                        game.HomeTeamScore = random.Next(0, 4); // Random home score
                        game.AwayTeamScore = random.Next(0, 4); // Random away score

                        // Update standings based on the game result
                        var homeStanding = standings.FirstOrDefault(s => s.TeamId == game.HomeTeamId && s.LeagueYear == league.Year);
                        var awayStanding = standings.FirstOrDefault(s => s.TeamId == game.AwayTeamId && s.LeagueYear == league.Year);
                        UpdateStandings(homeStanding, awayStanding, game.HomeTeamScore.Value, game.AwayTeamScore.Value);
                    }

                    // Add the game to the games collection
                    games.Add(game);
                }
            }

            _context.Games.AddRange(games);
            _context.Standings.AddRange(standings);
            _context.SaveChanges();
            */
        }
        List<Game> GenerateRoundRobinSchedule(List<Team> teams, int leagueYear, int scoredMatchDays, int totalMatchDays, DateTime leagueStartDate)
        {
            var games = new List<Game>();
            var faker = new Faker();
            int totalTeams = teams.Count;

            for (int round = 0; round < totalTeams - 1; round++) // Each team plays all others once
            {
                for (int match = 0; match < totalTeams / 2; match++) // Create matches for each pairing
                {
                    int homeIndex = (round + match) % (totalTeams - 1);
                    int awayIndex = (totalTeams - 1 - match + round) % (totalTeams - 1);

                    // Ensure the last team rotates correctly
                    if (match == 0)
                        awayIndex = totalTeams - 1;

                    var homeTeam = teams[homeIndex];
                    var awayTeam = teams[awayIndex];

                    // Generate the home game
                    games.Add(new Game
                    {
                        HomeTeamId = homeTeam.Id,
                        AwayTeamId = awayTeam.Id,
                        LeagueYear = leagueYear,
                        Date = leagueStartDate.AddDays(round*7),
                        MatchDay = round + 1,
                        HomeTeamScore = round + 1 <= scoredMatchDays ? faker.Random.Int(0, 5) : null,
                        AwayTeamScore = round + 1 <= scoredMatchDays ? faker.Random.Int(0, 5) : null
                    });

                    // Generate the reverse (away) game
                    games.Add(new Game
                    {
                        HomeTeamId = awayTeam.Id,
                        AwayTeamId = homeTeam.Id,
                        LeagueYear = leagueYear,
                        Date = leagueStartDate.AddDays((totalTeams - 1 + round) * 7),
                        MatchDay = totalTeams - 1 + round + 1,
                        HomeTeamScore = totalTeams - 1 + round + 1 <= scoredMatchDays ? faker.Random.Int(0, 5) : null,
                        AwayTeamScore = totalTeams - 1 + round + 1 <= scoredMatchDays ? faker.Random.Int(0, 5) : null
                    });
                }
            }

            // Truncate games to match the total number of matchdays
            return games.Where(g => g.MatchDay <= totalMatchDays).ToList();
        }

        void UpdateStandings(List<Game> games, List<Standing> standings)
        {
            foreach (var game in games.Where(g => g.HomeTeamScore.HasValue && g.AwayTeamScore.HasValue))
            {
                var homeStanding = standings.First(s => s.TeamId == game.HomeTeamId);
                var awayStanding = standings.First(s => s.TeamId == game.AwayTeamId);

                homeStanding.GamesPlayed++;
                awayStanding.GamesPlayed++;

                homeStanding.GoalsFor += game.HomeTeamScore.Value;
                homeStanding.GoalsAgainst += game.AwayTeamScore.Value;

                awayStanding.GoalsFor += game.AwayTeamScore.Value;
                awayStanding.GoalsAgainst += game.HomeTeamScore.Value;

                if (game.HomeTeamScore > game.AwayTeamScore)
                {
                    homeStanding.Wins++;
                    awayStanding.Losses++;
                }
                else if (game.HomeTeamScore < game.AwayTeamScore)
                {
                    awayStanding.Wins++;
                    homeStanding.Losses++;
                }
                else
                {
                    homeStanding.Draws++;
                    awayStanding.Draws++;
                }
            }
        }
        /*
        public void UpdateStandings(Standing homeTeamStanding, Standing awayTeamStanding, int homeScore, int awayScore)
        {
            homeTeamStanding.GamesPlayed++;
            awayTeamStanding.GamesPlayed++;
            homeTeamStanding.GoalsFor += homeScore;
            homeTeamStanding.GoalsAgainst += awayScore;
            awayTeamStanding.GoalsFor += awayScore;
            awayTeamStanding.GoalsAgainst += homeScore;

            if (homeScore > awayScore)
            {
                homeTeamStanding.Wins++;
                awayTeamStanding.Losses++;
            }
            else if (awayScore > homeScore)
            {
                awayTeamStanding.Wins++;
                homeTeamStanding.Losses++;
            }
            else
            {
                homeTeamStanding.Draws++;
                awayTeamStanding.Draws++;
            }
        }
        */
    }
}
