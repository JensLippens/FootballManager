using FootballManager.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class StandingDto
    {
        public int GamesPlayed { get; set; }
        public int Points { get; set; } // Add with Automapper Profile
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; } // Add with Automapper Profile
        public string TeamName { get; set; } = string.Empty; // Add with Automapper Profile
        public int LeagueYear { get; set; }
    }
}
