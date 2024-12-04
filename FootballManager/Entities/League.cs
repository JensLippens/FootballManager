using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballManager.Entities
{
    public class League
    {
        [Key]
        [Required]
        public int Year { get; set; }

        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Game> Games { get; set; } = new List<Game>();

        public League() { }

        public League(int year)
        {
            Year = year;
        }
    }
}
