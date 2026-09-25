using GiftOfTheGivers.Helpers;
using Xunit;

namespace GiftOfTheGivers.Tests;

public class DonationHelperTests
{
    [Fact]
    public void FormatCertificateReference_UsesDateAndPaddedDonationId()
    {
        var result = DonationHelper.FormatCertificateReference(42, new DateTime(2026, 8, 28));

        Assert.Equal("GOTG-20260828-000042", result);
    }

    [Fact]
    public void FormatCertificateReference_RejectsInvalidDonationId()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DonationHelper.FormatCertificateReference(0, DateTime.UtcNow));
    }

    [Fact]
    public void CalculateDonationTotal_SumsDonationAmounts()
    {
        var result = DonationHelper.CalculateDonationTotal(new[] { 125.50m, 74.50m, 10m });

        Assert.Equal(210m, result);
    }

    [Fact]
    public void CalculateDonationTotal_RejectsNegativeAmounts()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DonationHelper.CalculateDonationTotal(new[] { 100m, -1m }));
    }

    [Fact]
    public void FormatDonationAmount_UsesCurrencyAndTwoDecimals()
    {
        var result = DonationHelper.FormatDonationAmount(1250.5m, "zar");

        Assert.Equal("ZAR 1,250.50", result);
    }

    [Fact]
    public void FormatDonationAmount_RejectsMissingCurrency()
    {
        Assert.Throws<ArgumentException>(() => DonationHelper.FormatDonationAmount(10m, " "));
    }
}
