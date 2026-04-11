using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Models
{
    public class ResearchArea
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        // e.g. AI, Web Development, Cybersecurity

        public List<Project> Projects { get; set; } = new();
    }
}