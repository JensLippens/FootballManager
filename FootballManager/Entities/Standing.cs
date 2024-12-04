using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballManager.Entities
{
    public class Standing
    {
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }

        public int TeamId { get; set; }
        [ForeignKey("TeamId")]
        public Team? Team { get; set; }

        public int LeagueYear { get; set; }
        [ForeignKey("LeagueYear")]
        public League? League { get; set; }

        public Standing() { }

        public Standing(int teamId, int leagueYear)
        {
            TeamId = teamId;
            LeagueYear = leagueYear;
        }

        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Standing>()
                .HasKey(s => new { s.TeamId, s.LeagueYear });
        }
    }

}
