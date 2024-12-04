using FootballManager.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballManager.Entities
{
    public class Player : Person
    {
        [Required]
        public Position Position { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "The shirtnumber must be between 1 and 99.")]
        public int ShirtNumber { get; set; }

        public bool IsLeftFooted { get; set; } = false;

        [ForeignKey("TeamId")]
        public Team? Team { get; set; }
        public int? TeamId { get; set; }

        public Player() : base(string.Empty, string.Empty, DateTime.MinValue)
        { }

        public Player(string firstName, string lastName, DateTime dateOfBirth,
            Position position, int shirtNumber, bool isLeftFooted) : base(firstName, lastName, dateOfBirth)
        {
            Position = position;
            ShirtNumber = shirtNumber;
            IsLeftFooted = isLeftFooted;
        }
    }
}
