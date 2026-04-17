using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Abstract { get; set; } = string.Empty;

        public string TechStack { get; set; } = string.Empty;

        public int ResearchAreaId { get; set; }
        public ResearchArea? ResearchArea { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        public string Status { get; set; } = "Pending";
        public string? FilePath { get; set; }
        // Pending, UnderReview, Matched
    }
}