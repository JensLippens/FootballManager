using FootballManager.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class GameDto
    {
        public int Id { get; set; }
        public int MatchDay { get; set; }
        public string HomeTeamName { get; set; } = string.Empty; // See Automapper profile: HomeTeam.Name
        public string AwayTeamName { get; set; } = string.Empty; // See Automapper profile: AwayTeam.Name
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public string Score { get; set; } = string.Empty; // See Automapper profile: $"{HomeTeamScore} - {AwayTeamScore}"
        public DateTime Date { get; set; }
        public int SeasonYear { get; set; } // See Automapper profile: League.Year
    }
}
