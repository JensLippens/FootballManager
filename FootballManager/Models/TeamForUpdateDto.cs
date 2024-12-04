using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class TeamForUpdateDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "The team length must be shorter than 50 characters")]
        public string Name { get; set; } = string.Empty;
    }
}
