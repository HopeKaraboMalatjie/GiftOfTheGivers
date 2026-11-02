using System.Net;
using System.Text.RegularExpressions;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GiftOfTheGivers.Tests;

public sealed class DonationFlowIntegrationTests : IClassFixture<GiftOfTheGiversWebApplicationFactory>
{
    private readonly GiftOfTheGiversWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DonationFlowIntegrationTests(GiftOfTheGiversWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task AnonymousDonationIsSavedAndConfirmationIsReturned()
    {
        var formPage = await _client.GetAsync("/Donation/Create");
        Assert.Equal(HttpStatusCode.OK, formPage.StatusCode);

        var html = await formPage.Content.ReadAsStringAsync();
        var tokenMatch = Regex.Match(html, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(tokenMatch.Success, "The donation form must include an anti-forgery token.");

        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = tokenMatch.Groups[1].Value,
            ["Amount"] = "250",
            ["Currency"] = "ZAR",
            ["Frequency"] = "OneTime",
            ["guestName"] = "",
            ["guestEmail"] = ""
        });

        var submitResponse = await _client.PostAsync("/Donation/Create", form);

        Assert.Equal(HttpStatusCode.Redirect, submitResponse.StatusCode);
        Assert.NotNull(submitResponse.Headers.Location);
        Assert.StartsWith("/Donation/Certificate/", submitResponse.Headers.Location!.OriginalString);

        var confirmation = await _client.GetAsync(submitResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, confirmation.StatusCode);
        Assert.Contains("Thank You for Your Donation!", await confirmation.Content.ReadAsStringAsync());

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var donation = await db.Donations.SingleAsync();
        Assert.Equal(250m, donation.Amount);
        Assert.Equal(Currency.ZAR, donation.Currency);
        Assert.Equal("Anonymous Donor", donation.GuestName);
        Assert.True(donation.CertificateGenerated);
    }
}
