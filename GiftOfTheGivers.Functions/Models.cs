using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Functions;

public sealed class DonationCertificateRequest
{
    [Required, StringLength(150)]
    public string DonorName { get; set; } = string.Empty;

    [EmailAddress, StringLength(254)]
    public string? DonorEmail { get; set; }

    [Range(typeof(decimal), "0.01", "1000000000")]
    public decimal Amount { get; set; }

    [Required, StringLength(100)]
    public string DonationReference { get; set; } = string.Empty;

    [StringLength(200)]
    public string? ProjectName { get; set; }

    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "ZAR";

    public DateTime? DonationDate { get; set; }
}

public sealed class EmployeeUpdateRequest
{
    [Required, StringLength(150)]
    public string EmployeeEmail { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string ProjectTitle { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Status { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }
}
