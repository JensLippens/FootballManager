using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballManager.Entities
{
    public class Coach : Person
    {
        [Required]
        public int YearsOfExperience { get; set; }

        [ForeignKey("TeamId")]
        public Team? Team { get; set; }
        public int TeamId { get; set; }

        public Coach() : base(string.Empty, string.Empty, DateTime.MinValue)
        { }

        public Coach(string firstName, string lastName, DateTime dateOfBirth,
            int yearsOfExperience) : base(firstName, lastName, dateOfBirth)
        {
            YearsOfExperience = yearsOfExperience;
        }
    }
}
