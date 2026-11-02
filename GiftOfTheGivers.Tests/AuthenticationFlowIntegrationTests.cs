using System.Net;
using System.Text.RegularExpressions;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GiftOfTheGivers.Tests;

public sealed class AuthenticationFlowIntegrationTests : IClassFixture<GiftOfTheGiversWebApplicationFactory>
{
    private readonly GiftOfTheGiversWebApplicationFactory _factory;

    public AuthenticationFlowIntegrationTests(GiftOfTheGiversWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/Identity/Account/Login")]
    [InlineData("/Identity/Account/Register")]
    public async Task AccountPagesUseSharedGiftOfTheGiversLayout(string path)
    {
        using var client = CreateClient();
        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("gotg-navbar", html);
        Assert.Contains("gotg-logo-mark", html);
        Assert.Contains("/css/site.css", html);
        Assert.Contains("account-card", html);
    }

    [Fact]
    public async Task RegistrationCreatesDonorAccountAndSignsUserIn()
    {
        const string email = "new-donor@example.org";
        await EnsureDonorRoleExistsAsync();
        using var client = CreateClient();

        var token = await GetAntiforgeryTokenAsync(client, "/Identity/Account/Register");
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Input.FullName"] = "New Donor",
            ["Input.Email"] = email,
            ["Input.Password"] = "Donor@123",
            ["Input.ConfirmPassword"] = "Donor@123"
        });

        var response = await client.PostAsync("/Identity/Account/Register", form);

        Assert.True(response.StatusCode == HttpStatusCode.Redirect,
            $"Expected registration redirect, got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        Assert.Equal("/", response.Headers.Location?.OriginalString);

        var homePage = await client.GetAsync("/");
        Assert.Contains(email, await homePage.Content.ReadAsStringAsync());

        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        Assert.True(await userManager.IsInRoleAsync(user!, "Donor"));
    }

    [Fact]
    public async Task LoginWithValidCredentialsRedirectsHome()
    {
        const string email = "existing-donor@example.org";
        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser { UserName = email, Email = email, FullName = "Existing Donor", EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, "Donor@123");
            Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(error => error.Description)));
        }

        using var client = CreateClient();
        var token = await GetAntiforgeryTokenAsync(client, "/Identity/Account/Login");
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Input.Email"] = email,
            ["Input.Password"] = "Donor@123"
        });

        var response = await client.PostAsync("/Identity/Account/Login", form);

        Assert.True(response.StatusCode == HttpStatusCode.Redirect,
            $"Expected login redirect, got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        Assert.Equal("/", response.Headers.Location?.OriginalString);
        var homePage = await client.GetAsync("/");
        Assert.Contains(email, await homePage.Content.ReadAsStringAsync());
    }

    private HttpClient CreateClient() => _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });

    private async Task EnsureDonorRoleExistsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync("Donor"))
        {
            var result = await roleManager.CreateAsync(new IdentityRole("Donor"));
            Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }

    private static async Task<string> GetAntiforgeryTokenAsync(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(html, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(match.Success, $"The form at {path} must include an anti-forgery token.");
        return match.Groups[1].Value;
    }
}
