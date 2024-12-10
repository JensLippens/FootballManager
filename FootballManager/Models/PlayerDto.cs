namespace FootballManager.Models
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? FullName { get; set; } // => $"{FirstName} {LastName}", see AutoMapper profile
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; } // Calculated field in AutoMapper profile
        public Position Position { get; set; }
        public bool IsLeftFooted { get; set; } = false;
        public int ShirtNumber { get; set; }
        public string TeamName { get; set; } = string.Empty;
    }

    public enum Position
    {
        Goalkeeper,
        Defender,
        Midfielder,
        Striker
    }
}

