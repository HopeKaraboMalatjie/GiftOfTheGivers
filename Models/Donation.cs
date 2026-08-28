using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public enum DonationFrequency
    {
        OneTime,
        Recurring
    }

    public enum Currency
    {
        ZAR,
        USD,
        EUR
    }

    public class Donation
    {
        public int DonationId { get; set; }

        // Null = anonymous guest donor
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        [StringLength(100)]
        public string? GuestName { get; set; }

        [StringLength(100), EmailAddress]
        public string? GuestEmail { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(1, 1000000)]
        public decimal Amount { get; set; }

        public Currency Currency { get; set; } = Currency.ZAR;
        public DonationFrequency Frequency { get; set; } = DonationFrequency.OneTime;

        public DateTime DonatedOn { get; set; } = DateTime.UtcNow;

        // Optional link to the relief project the donation supports - FK
        public int? ReliefProjectId { get; set; }
        public ReliefProject? ReliefProject { get; set; }

        public bool CertificateGenerated { get; set; } = false;
    }
}
