using System;
using System.Collections.Generic;
using System.Linq;

namespace GiftOfTheGivers.Helpers
{
    public static class DonationHelpers
    {
        public static string FormatTaxCertificateNumber(int donationId, DateTime donationDate)
        {
            if (donationId <= 0)
                throw new ArgumentOutOfRangeException(nameof(donationId), "Donation ID must be a positive integer.");

            var year = donationDate.Year;
            return $"GOTG-{year}-{donationId:D6}";
        }

        public static decimal CalculateDonationTotal(IEnumerable<decimal> amounts)
        {
            if (amounts is null) throw new ArgumentNullException(nameof(amounts));
            return amounts.Sum();
        }

        public static decimal CalculateAverageDonation(IEnumerable<decimal> amounts)
        {
            if (amounts is null) throw new ArgumentNullException(nameof(amounts));
            var list = amounts as IList<decimal> ?? amounts.ToList();
            if (!list.Any()) return 0m;
            return list.Average();
        }
    }
}
