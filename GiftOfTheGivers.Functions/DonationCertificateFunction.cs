using System.ComponentModel.DataAnnotations;
using GiftOfTheGivers.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace GiftOfTheGivers.Functions;

public sealed class DonationCertificateFunction
{
    private readonly ILogger<DonationCertificateFunction> _logger;

    public DonationCertificateFunction(ILogger<DonationCertificateFunction> logger)
    {
        _logger = logger;
    }

    [Function("DonationCertificateFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "donationcertificate")] HttpRequestData request)
    {
        if (request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            var healthResponse = request.CreateResponse(System.Net.HttpStatusCode.OK);
            await healthResponse.WriteAsJsonAsync(new
            {
                function = "DonationCertificateFunction",
                status = "ready",
                method = "POST generates the demonstration PDF certificate"
            });
            return healthResponse;
        }

        try
        {
            var input = await request.ReadFromJsonAsync<DonationCertificateRequest>();
            if (input is null)
            {
                return await ErrorResponseAsync(request, System.Net.HttpStatusCode.BadRequest, "A JSON request body is required.");
            }

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(input, new ValidationContext(input), validationResults, true))
            {
                var response = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await response.WriteAsJsonAsync(new
                {
                    error = "Validation failed.",
                    details = validationResults.Select(result => result.ErrorMessage).ToArray()
                });
                return response;
            }

            var donationDate = input.DonationDate ?? DateTime.UtcNow;
            var formattedAmount = DonationHelper.FormatDonationAmount(input.Amount, input.Currency);
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(text => text.FontSize(12));
                    page.Header().Text("Gift of the Givers Foundation").SemiBold().FontSize(20).FontColor(Colors.Red.Darken2);
                    page.Content().Column(column =>
                    {
                        column.Spacing(10);
                        column.Item().PaddingTop(15).Text("DEMONSTRATION DONATION CERTIFICATE").SemiBold().FontSize(16);
                        column.Item().Text("DUMMY DOCUMENT - NOT A LEGALLY VALID SARS Section 18A TAX CERTIFICATE.").Bold().FontColor(Colors.Red.Darken2);
                        column.Item().Text($"Donation reference: {input.DonationReference}");
                        column.Item().Text($"Donor: {input.DonorName}");
                        column.Item().Text($"Date: {donationDate:dd MMMM yyyy}");
                        column.Item().Text($"Amount: {formattedAmount}");
                        column.Item().Text($"Project: {input.ProjectName ?? "General Fund"}");
                        column.Item().PaddingTop(20).Text("This academic prototype document confirms receipt of the submitted donation information only.");
                    });
                    page.Footer().AlignCenter().Text("Gift of the Givers Foundation - Academic Prototype").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            }).GeneratePdf();

            _logger.LogInformation("Generated demonstration certificate for donation {Reference}.", input.DonationReference);
            var result = request.CreateResponse(System.Net.HttpStatusCode.OK);
            result.Headers.Add("Content-Disposition", $"attachment; filename=GOTG-Demonstration-{input.DonationReference}.pdf");
            result.Headers.Add("Content-Type", "application/pdf");
            await result.Body.WriteAsync(pdf);
            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Donation certificate generation failed.");
            return await ErrorResponseAsync(request, System.Net.HttpStatusCode.InternalServerError, "The demonstration certificate could not be generated.");
        }
    }

    private static async Task<HttpResponseData> ErrorResponseAsync(HttpRequestData request, System.Net.HttpStatusCode statusCode, string message)
    {
        var response = request.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }
}
