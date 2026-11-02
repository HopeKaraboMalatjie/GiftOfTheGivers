using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace GiftOfTheGivers.Tests;

public sealed class SeleniumUiTests
{
    [SiteConfiguredFact]
    public void LoginPageDisplaysSignInForm()
    {
        using var driver = CreateDriver();
        driver.Navigate().GoToUrl(new Uri(new Uri(GetBaseUrl()), "/Identity/Account/Login"));

        Assert.True(driver.FindElement(By.Id("Input_Email")).Displayed);
        Assert.True(driver.FindElement(By.Id("Input_Password")).Displayed);
        SaveScreenshot(driver, "login-page");
    }

    [DonationWriteEnabledFact]
    public void AnonymousDonorCanSubmitDonation()
    {
        using var driver = CreateDriver();
        driver.Navigate().GoToUrl(new Uri(new Uri(GetBaseUrl()), "/Donation/Create"));

        var amount = driver.FindElement(By.Name("Amount"));
        amount.Clear();
        amount.SendKeys("10");
        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        var pageText = driver.FindElement(By.TagName("body")).Text;
        var validationErrors = driver.FindElements(By.CssSelector(".field-validation-error"))
            .Select(error => error.Text)
            .Where(error => !string.IsNullOrWhiteSpace(error));
        Assert.True(pageText.Contains("Thank You for Your Donation"),
            $"Donation confirmation was not displayed. Validation errors: {string.Join(" | ", validationErrors)}");
        SaveScreenshot(driver, "anonymous-donation-confirmation");
    }

    private static ChromeDriver CreateDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--window-size=1280,1200");
        return new ChromeDriver(options);
    }

    private static string GetBaseUrl() =>
        Environment.GetEnvironmentVariable("GOTG_TEST_BASE_URL")!.TrimEnd('/') + "/";

    private static void SaveScreenshot(IWebDriver driver, string name)
    {
        var directory = Environment.GetEnvironmentVariable("GOTG_UI_SCREENSHOT_DIR");
        if (string.IsNullOrWhiteSpace(directory))
        {
            return;
        }

        Directory.CreateDirectory(directory);
        ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(Path.Combine(directory, $"{name}.png"));
    }
}
