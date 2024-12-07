using FootballManager.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class LeagueWithTeamsAndGamesDto
    {
        public int Year { get; set; }
        public ICollection<TeamWithoutGamesOrPlayersDto> Teams { get; set; } = new List<TeamWithoutGamesOrPlayersDto>();
        public ICollection<GameDto> Games { get; set; } = new List<GameDto>();
    }
}
