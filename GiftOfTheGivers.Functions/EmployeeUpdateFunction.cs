using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GiftOfTheGivers.Functions;

public sealed class EmployeeUpdateFunction
{
    private readonly ILogger<EmployeeUpdateFunction> _logger;

    public EmployeeUpdateFunction(ILogger<EmployeeUpdateFunction> logger)
    {
        _logger = logger;
    }

    [Function("EmployeeUpdateFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "employeeupdate")] HttpRequestData request)
    {
        try
        {
            var input = await request.ReadFromJsonAsync<EmployeeUpdateRequest>();
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

            _logger.LogInformation("Employee update received from {EmployeeEmail} for project {ProjectTitle} with status {Status}.", input.EmployeeEmail, input.ProjectTitle, input.Status);
            var accepted = request.CreateResponse(System.Net.HttpStatusCode.Accepted);
            await accepted.WriteAsJsonAsync(new
            {
                message = "Employee update received.",
                input.ProjectTitle,
                input.Status,
                receivedAt = DateTime.UtcNow
            });
            return accepted;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Employee update processing failed.");
            return await ErrorResponseAsync(request, System.Net.HttpStatusCode.InternalServerError, "The employee update could not be processed.");
        }
    }

    private static async Task<HttpResponseData> ErrorResponseAsync(HttpRequestData request, System.Net.HttpStatusCode statusCode, string message)
    {
        var response = request.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }
}
