using Xunit;

namespace GiftOfTheGivers.Tests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SiteConfiguredFactAttribute : FactAttribute
{
    public SiteConfiguredFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GOTG_TEST_BASE_URL")))
        {
            Skip = "Set GOTG_TEST_BASE_URL to run Selenium UI tests against a test site.";
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class DonationWriteEnabledFactAttribute : FactAttribute
{
    public DonationWriteEnabledFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GOTG_TEST_BASE_URL")) ||
            !string.Equals(Environment.GetEnvironmentVariable("GOTG_TEST_ALLOW_DONATION_WRITE"), "true", StringComparison.OrdinalIgnoreCase))
        {
            Skip = "Set GOTG_TEST_BASE_URL and GOTG_TEST_ALLOW_DONATION_WRITE=true to submit a donation to an isolated test site.";
        }
    }
}
