using System.Net.Http.Json;
using GiftOfTheGivers.Models;
using Microsoft.Extensions.Options;

namespace GiftOfTheGivers.Services;

public sealed class AzureFunctionsOptions
{
    public string BaseUrl { get; set; } = "http://localhost:7071";
}

public interface IAzureFunctionClient
{
    Task<byte[]?> GenerateDonationCertificateAsync(Donation donation, string donorName, string donationReference, CancellationToken cancellationToken = default);
    Task<bool> NotifyEmployeeUpdateAsync(ReliefProject project, string employeeEmail, CancellationToken cancellationToken = default);
}

public sealed class AzureFunctionClient : IAzureFunctionClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AzureFunctionClient> _logger;

    public AzureFunctionClient(HttpClient httpClient, IOptions<AzureFunctionsOptions> options, ILogger<AzureFunctionClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress ??= new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
        _logger = logger;
    }

    public async Task<byte[]?> GenerateDonationCertificateAsync(Donation donation, string donorName, string donationReference, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            donorName,
            donorEmail = donation.ApplicationUser?.Email ?? donation.GuestEmail,
            amount = donation.Amount,
            donationReference,
            projectName = donation.ReliefProject?.Title,
            currency = donation.Currency.ToString(),
            donationDate = donation.DonatedOn
        };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/donationcertificate", payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Donation certificate Function returned {StatusCode}: {Error}", response.StatusCode, error);
                return null;
            }

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Donation certificate Function is unavailable at {BaseAddress}; using the local certificate generator.", _httpClient.BaseAddress);
            return null;
        }
    }

    public async Task<bool> NotifyEmployeeUpdateAsync(ReliefProject project, string employeeEmail, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            employeeEmail,
            projectTitle = project.Title,
            project.Status,
            project.Description
        };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/employeeupdate", payload, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Employee update Function accepted project update {ProjectTitle}.", project.Title);
                return true;
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Employee update Function returned {StatusCode}: {Error}", response.StatusCode, error);
            return false;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Employee update Function is unavailable at {BaseAddress}; database update remains successful.", _httpClient.BaseAddress);
            return false;
        }
    }
}
