using System.Globalization;

namespace GiftOfTheGivers.Helpers;

public static class DonationHelper
{
    public static string FormatCertificateReference(int donationId, DateTime donatedOn)
    {
        if (donationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(donationId), "Donation ID must be greater than zero.");
        }

        return $"GOTG-{donatedOn:yyyyMMdd}-{donationId:D6}";
    }

    public static decimal CalculateDonationTotal(IEnumerable<decimal> amounts)
    {
        ArgumentNullException.ThrowIfNull(amounts);

        var total = 0m;
        foreach (var amount in amounts)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amounts), "Donation amounts cannot be negative.");
            }

            total += amount;
        }

        return total;
    }

    public static string FormatDonationAmount(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Donation amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("A currency code is required.", nameof(currency));
        }

        return $"{currency.Trim().ToUpperInvariant()} {amount.ToString("N2", CultureInfo.InvariantCulture)}";
    }
}
