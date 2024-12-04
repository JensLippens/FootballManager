using FootballManager.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class LeagueWithGamesDto
    {
        public int Year { get; set; }
        public ICollection<GameDto> Games { get; set; } = new List<GameDto>();
    }
}
