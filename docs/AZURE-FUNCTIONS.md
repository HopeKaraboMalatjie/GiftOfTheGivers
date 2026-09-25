# Azure Functions

## Local project

`GiftOfTheGivers.Functions` targets .NET 8 and Azure Functions v4 using the isolated worker model. Install Azure Functions Core Tools v4, copy `local.settings.example.json` to `local.settings.json`, and run:

```powershell
cd GiftOfTheGivers.Functions
func start --dotnet-isolated-debug
```

The local host is normally `http://localhost:7071`.

## Endpoints

Health/status request:

```http
GET http://localhost:7071/api/donationcertificate
```

Generate a demonstration PDF (Postman or curl):

```http
POST http://localhost:7071/api/donationcertificate
Content-Type: application/json

{
  "donorName": "Demo Donor",
  "donorEmail": "donor@example.org",
  "amount": 500,
  "donationReference": "GOTG-20260828-000001",
  "projectName": "Flood Relief - KwaZulu-Natal",
  "currency": "ZAR",
  "donationDate": "2026-08-28T12:00:00Z"
}
```

The response is a PDF explicitly marked as a demonstration/dummy document and not a legally valid SARS Section 18A certificate.

Employee update:

```http
POST http://localhost:7071/api/employeeupdate
Content-Type: application/json

{
  "employeeEmail": "employee@giftofthegivers.org",
  "projectTitle": "Flood Relief - KwaZulu-Natal",
  "status": "Active",
  "description": "Water and food distribution update"
}
```

## Web integration

The MVC application uses `Services/AzureFunctionClient.cs`, a typed `HttpClient` registered in `Program.cs`. `AzureFunctions:BaseUrl` is configured through appsettings or environment variables. Donation certificate downloads call the Function first and use the existing QuestPDF generator if the Function is unavailable. Employee project creation saves to the database first and then sends a non-blocking notification.

## Azure deployment

Create a Function App configured for .NET 8 isolated worker, configure `FUNCTIONS_WORKER_RUNTIME=dotnet-isolated`, deploy the published Functions artifact, and set the web application's `AzureFunctions__BaseUrl` setting to the deployed HTTPS base URL. Deployment and successful Azure execution require the user's Azure subscription and are not claimed here.
