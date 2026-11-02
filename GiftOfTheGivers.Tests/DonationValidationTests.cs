using System.ComponentModel.DataAnnotations;
using GiftOfTheGivers.Models;
using Xunit;

namespace GiftOfTheGivers.Tests;

public sealed class DonationValidationTests
{
    [Fact]
    public void DonationWithValidAmountAndEmailPassesValidation()
    {
        var donation = new Donation { Amount = 125.50m, GuestEmail = "donor@example.org" };

        Assert.Empty(Validate(donation));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000001)]
    public void DonationOutsideAllowedAmountRangeFailsValidation(decimal amount)
    {
        var donation = new Donation { Amount = amount };

        Assert.Contains(Validate(donation), result => result.MemberNames.Contains(nameof(Donation.Amount)));
    }

    [Fact]
    public void InvalidGuestEmailFailsValidation()
    {
        var donation = new Donation { Amount = 50m, GuestEmail = "not-an-email" };

        Assert.Contains(Validate(donation), result => result.MemberNames.Contains(nameof(Donation.GuestEmail)));
    }

    private static IReadOnlyCollection<ValidationResult> Validate(Donation donation)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(donation, new ValidationContext(donation), results, validateAllProperties: true);
        return results;
    }
}
