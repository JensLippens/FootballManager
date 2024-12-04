namespace FootballManager.Models
{
    public class CoachDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}"; // TODO: change to AutoMapper profile
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; } // TODO: change to AutoMapper profile
        public int YearsOfExperience { get; set; }
        public string TeamName {get; set; } = string.Empty;
    }

}
