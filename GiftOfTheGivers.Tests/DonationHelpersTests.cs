using System;
using System.Collections.Generic;
using GiftOfTheGivers.Helpers;
using Xunit;

namespace GiftOfTheGivers.Tests
{
    public class DonationHelpersTests
    {
        [global::Xunit.Fact]
        public void FormatTaxCertificateNumber_ValidInputs_ReturnsExpected()
        {
            var cert = DonationHelpers.FormatTaxCertificateNumber(42, new DateTime(2024, 5, 1));
            Assert.Equal("GOTG-2024-000042", cert);
        }

        [global::Xunit.Fact]
        public void FormatTaxCertificateNumber_InvalidId_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DonationHelpers.FormatTaxCertificateNumber(0, DateTime.UtcNow));
        }

        [global::Xunit.Fact]
        public void CalculateDonationTotal_MultipleAmounts_SumsCorrectly()
        {
            var amounts = new decimal[] { 10.5m, 20m, 5.25m };
            var total = DonationHelpers.CalculateDonationTotal(amounts);
            Assert.Equal(35.75m, total);
        }

        [global::Xunit.Fact]
        public void CalculateDonationTotal_EmptyCollection_ReturnsZero()
        {
            var amounts = new decimal[0];
            var total = DonationHelpers.CalculateDonationTotal(amounts);
            Assert.Equal(0m, total);
        }

        [global::Xunit.Fact]
        public void CalculateAverageDonation_MultipleAmounts_ReturnsAverage()
        {
            var amounts = new decimal[] { 10m, 20m, 30m };
            var avg = DonationHelpers.CalculateAverageDonation(amounts);
            Assert.Equal(20m, avg);
        }
    }
}
