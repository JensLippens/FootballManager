using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class GameForUpdateDto
    {
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
    }
}
