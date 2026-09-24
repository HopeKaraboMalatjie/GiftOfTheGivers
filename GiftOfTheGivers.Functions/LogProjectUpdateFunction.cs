using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GiftOfTheGivers.Functions
{
    public class LogProjectUpdateFunction
    {
        private readonly ILogger _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public LogProjectUpdateFunction(ILoggerFactory loggerFactory, BlobServiceClient blobServiceClient)
        {
            _logger = loggerFactory.CreateLogger<LogProjectUpdateFunction>();
            _blobServiceClient = blobServiceClient;
        }

        [global::Microsoft.Azure.Functions.Worker.Function("LogProjectUpdate")]
        public async Task<HttpResponseData> Run(
            [global::Microsoft.Azure.Functions.Worker.Http.HttpTrigger(global::Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "post", Route = "project/update")] HttpRequestData req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string containerName = Environment.GetEnvironmentVariable("PROJECT_UPDATES_CONTAINER") ?? "project-updates";
            var utcNow = DateTime.UtcNow;

            try
            {
                using var doc = JsonDocument.Parse(requestBody);
                var root = doc.RootElement;
                var projectId = root.GetProperty("reliefProjectId").GetInt32();
                _logger.LogInformation("Logging project update for projectId={ProjectId}", projectId);

                var update = new
                {
                    reliefProjectId = projectId,
                    title = root.GetProperty("title").GetString(),
                    status = root.GetProperty("status").GetString(),
                    updatedBy = root.GetProperty("updatedBy").GetString(),
                    updatedUtc = utcNow
                };

                var payload = JsonSerializer.Serialize(update);

                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();

                var blobName = $"updates/{projectId}/{Guid.NewGuid()}.json";
                var blobClient = containerClient.GetBlobClient(blobName);

                using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payload));
                await blobClient.UploadAsync(ms, overwrite: true);

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteStringAsync($"Project update logged to blob: {blobName}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log project update");
                var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await response.WriteStringAsync("Invalid request or failed to log update.");
                return response;
            }
        }
    }
}
