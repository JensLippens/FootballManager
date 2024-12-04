using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class GameForUpdateDto
    {
        [Required]
        public int MatchDay { get; set; }

        [Required]
        public int HomeTeamId { get; set; }

        [Required]
        public int AwayTeamId { get; set; }

        [Required]
        public int LeagueYear { get; set; }

        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
