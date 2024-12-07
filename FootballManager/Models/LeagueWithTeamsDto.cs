using FootballManager.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class LeagueWithTeamsDto
    {
        public int Year { get; set; }
        public ICollection<TeamWithoutGamesOrPlayersDto> Teams { get; set; } = new List<TeamWithoutGamesOrPlayersDto>();       
    }
}
