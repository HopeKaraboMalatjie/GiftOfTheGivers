using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using GiftOfTheGivers.Helpers;

namespace GiftOfTheGivers.Functions
{
    public class GenerateTaxCertificateFunction
    {
        private readonly ILogger _logger;

        public GenerateTaxCertificateFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GenerateTaxCertificateFunction>();
        }

        [global::Microsoft.Azure.Functions.Worker.Function("GenerateTaxCertificate")]
        public async Task<HttpResponseData> Run(
            [global::Microsoft.Azure.Functions.Worker.Http.HttpTrigger(global::Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "post", Route = "taxcertificate/{donationId:int}")] HttpRequestData req,
            int donationId)
        {
            _logger.LogInformation("GenerateTaxCertificate triggered for donationId={DonationId}", donationId);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string donorName = "Anonymous Donor";
            decimal donationAmount = 0m;

            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                try
                {
                    using var doc = JsonDocument.Parse(requestBody);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("donorName", out var dn)) donorName = dn.GetString() ?? donorName;
                    if (root.TryGetProperty("donationAmount", out var da)) donationAmount = da.GetDecimal();
                }
                catch (JsonException)
                {
                    // ignore and use defaults
                }
            }

            var utcNow = DateTime.UtcNow;
            var certificate = DonationHelpers.FormatTaxCertificateNumber(donationId, utcNow);

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var payload = new
            {
                certificateNumber = certificate,
                donorName,
                donationAmount,
                generatedUtc = utcNow
            };

            await response.WriteStringAsync(JsonSerializer.Serialize(payload));
            return response;
        }
    }
}
