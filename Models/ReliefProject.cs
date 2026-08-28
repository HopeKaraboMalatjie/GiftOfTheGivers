using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Models
{
    public class ReliefProject
    {
        public int ReliefProjectId { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Completed, Suspended

        // Employee who posted/owns the update - FK to ApplicationUser
        public string? PostedByUserId { get; set; }
        public ApplicationUser? PostedByUser { get; set; }

        public ICollection<VolunteerSignup> VolunteerSignups { get; set; } = new List<VolunteerSignup>();
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}
