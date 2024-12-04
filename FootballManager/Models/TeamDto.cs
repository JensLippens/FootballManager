using FootballManager.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CoachDto? Coach { get; set; }
        public ICollection<PlayerDto> Players { get; set; } = new List<PlayerDto>();
        public ICollection<GameDto> Games { get; set; } = new List<GameDto>();
        public ICollection<LeagueDto> Leagues { get; set; } = new List<LeagueDto>();
        //public StandingDto? Standing { get; set; }       
    }
}
