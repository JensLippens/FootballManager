using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class LeagueForCreationDto
    {
        [Required]
        public int Year { get; set; }
    }
}
