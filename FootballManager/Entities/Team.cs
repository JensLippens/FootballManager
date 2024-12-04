using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballManager.Entities
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The team length must be shorter than 50 characters")]
        public string Name { get; set; }
        public Coach? Coach { get; set; }
        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<Game> Games { get; set; } = new List<Game>();
        
        public ICollection<League> Leagues { get; set; } = new List<League>();
               
        /*
        public Standing? Standing { get; set; }
             
        [ForeignKey("LeagueId")]
        public League? League { get; set; }
        public int LeagueId { get; set; }    
        */


        public Team() { }
        
        public Team(string name)
        {
            Name = name;
        }

        /*
        public Team(string name, int leagueId)
        {
            Name = name;
            LeagueId = leagueId;
        }
        */
    }
}
