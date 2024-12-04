using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballManager.Entities
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int MatchDay { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }

        [ForeignKey("HomeTeamId")]
        public Team? HomeTeam { get; set; }
        [Required]
        public int HomeTeamId { get; set; }

        [ForeignKey("AwayTeamId")]
        public Team? AwayTeam { get; set; }
        [Required]
        public int AwayTeamId { get; set; }

        [Required]
        public int LeagueYear { get; set; }
        [ForeignKey("LeagueYear")]
        public League? League { get; set; }

        public Game()
        { }

        public Game(int matchDay, int homeTeamId, int awayTeamId, DateTime date, int leagueYear)
        {
            MatchDay = matchDay;
            HomeTeamId = homeTeamId;
            AwayTeamId = awayTeamId;
            Date = date;
            LeagueYear = leagueYear;
        }
    }
}
