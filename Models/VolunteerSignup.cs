using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Models
{
    public class VolunteerSignup
    {
        public int VolunteerSignupId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [Required, StringLength(300)]
        public string Skills { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Availability { get; set; } = string.Empty; // e.g. "Weekends", "Full-time"

        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;

        // Optional link to a specific relief project - FK
        public int? ReliefProjectId { get; set; }
        public ReliefProject? ReliefProject { get; set; }

        // Optional link if the volunteer registered while logged in as a Donor
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
