# Part 2 setup and demonstration guide

## 1. Open, restore, build, and test

1. Open `GiftOfTheGivers.sln` in Visual Studio.
2. Restore packages with **Build > Restore NuGet Packages**, or run:
   ```powershell
   dotnet restore GiftOfTheGivers.sln
   dotnet build GiftOfTheGivers.sln
   dotnet test GiftOfTheGivers.Tests/GiftOfTheGivers.Tests.csproj
   ```
3. The web project uses the existing EF SQL Server configuration. Apply existing migrations with `dotnet ef database update` when a valid database connection is configured; do not use `EnsureCreated`.

## 2. Run the MVC application

Run `GiftOfTheGivers` from Visual Studio. Configure the Development SQL Server/LocalDB connection in `appsettings.Development.json` or user secrets. The web app reads `AzureFunctions:BaseUrl` from configuration and defaults to `http://localhost:7071` in Development.

## 3. Run and test Functions locally

Install Azure Functions Core Tools v4, copy `GiftOfTheGivers.Functions/local.settings.example.json` to `local.settings.json`, and run:

```powershell
cd GiftOfTheGivers.Functions
func start
```

Test `GET http://localhost:7071/api/donationcertificate` in a browser. Use Postman for the POST examples in `docs/AZURE-FUNCTIONS.md`. Save the returned PDF and verify the visible dummy/SARS disclaimer. Post the employee JSON example to `/api/employeeupdate` and verify the 202 response and terminal log.

## 4. Test existing workflows

- Donation: open `/Donation/Create`, submit a valid donation, confirm the database record and open the certificate download. With Functions running, the generated PDF comes from the Function; with Functions stopped, the existing local QuestPDF fallback is used.
- Employee: log in as the seeded Employee account, open the employee dashboard, post a project update, confirm the database update succeeds, and inspect the Function log/response when the Function is running.
- Volunteer: open `/Volunteer/Create`, submit the existing volunteer form, and confirm the existing confirmation page and database record remain working.
- Identity: use `/Identity/Account/Register` and `/Identity/Account/Login` with a valid database.

## 5. Azure Repos and branches

Follow `docs/AZURE-REPOS.md` to create the DevOps project/repository, connect Visual Studio, create `donations-feature` and `volunteer-feature`, commit meaningful changes, push, open pull requests, merge into `main`, and verify history. Azure account access is required.

## 6. Azure Pipeline

Follow `docs/AZURE-PIPELINES.md`. Create a pipeline from the existing `azure-pipelines.yml`, run it, verify restore/build/test/publish tasks and web, Functions, and helper-package artifacts. Configure an Azure Resource Manager service connection only if deployment tasks are added.

## 7. Azure Artifacts/NuGet

Follow `docs/AZURE-ARTIFACTS.md`. Create a feed, configure authenticated NuGet access using `NuGet.config.template`, publish the package, and optionally change the MVC ProjectReference to a PackageReference after the feed is available.

## 8. Publish the Function

Create an Azure Function App for .NET 8 isolated worker, deploy the Functions artifact or use the pipeline, configure `FUNCTIONS_WORKER_RUNTIME=dotnet-isolated`, and set the web app's `AzureFunctions__BaseUrl` to the deployed Function App URL. Test both endpoints after deployment.

## 9. Evidence screenshots

Use `docs/PART2-EVIDENCE-CHECKLIST.md`. Capture only real screens from your Visual Studio, local runtime, Azure DevOps, and Azure portal; do not fabricate evidence.
