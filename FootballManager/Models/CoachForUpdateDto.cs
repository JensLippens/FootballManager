using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class CoachForUpdateDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "The first name length must be shorter than 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50, ErrorMessage = "The last name length must be shorter than 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "The years of experience must be between 1 and 99.")]
        public int YearsOfExperience { get; set; }

        public int? TeamId { get; set; }
    }

}
