using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public string SupervisorId { get; set; } = string.Empty;
        public ApplicationUser? Supervisor { get; set; }

        public bool IsRevealed { get; set; } = false;
        // false = Blind, true = Identity Revealed

        public DateTime MatchedOn { get; set; } = DateTime.Now;
    }
}
