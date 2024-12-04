using FootballManager.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class LeagueWithTeamsAndGamesDto
    {
        public int Year { get; set; }
        public ICollection<TeamDto> Teams { get; set; } = new List<TeamDto>();
        public ICollection<GameDto> Games { get; set; } = new List<GameDto>();
    }
}
