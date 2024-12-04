using FootballManager.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class TeamWithoutGamesOrPlayersDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        //public StandingDto? Standing { get; set; }       
    }
}
