# Gift of the Givers Foundation

.NET 8 ASP.NET Core MVC/Razor Pages prototype for relief projects, donations, volunteers, and employee updates.

## Solution projects

- `GiftOfTheGivers` - existing web application.
- `GiftOfTheGivers.Functions` - .NET 8 Azure Functions v4 isolated-worker HTTP endpoints.
- `GiftOfTheGivers.Helpers` - reusable donation/certificate helper library and NuGet package.
- `GiftOfTheGivers.Tests` - xUnit unit and HTTP integration tests, plus opt-in Selenium UI tests.

## Local development

1. Open `GiftOfTheGivers.sln` in Visual Studio.
2. Restore and build the solution.
3. Configure the Development connection string in `appsettings.Development.json` or user secrets, then run the web project and EF migrations. Production connection strings must be supplied through secure environment/App Service configuration (`ConnectionStrings__DefaultConnection`).
4. To use the Function integration, run `GiftOfTheGivers.Functions` with Azure Functions Core Tools on port 7071. The web app defaults to `http://localhost:7071` in Development and falls back to its existing local certificate generator if the Function is unavailable.
5. See `PART2-SETUP.md` for local setup, `docs/AZURE-PIPELINES.md` for Part 3 CI/CD setup, and `docs/USER-MANUAL.md` for application workflows.

Part 3 handover resources: `docs/PART3-EVIDENCE-CHECKLIST.md` and `docs/TEAM-CONTRIBUTION-REPORT.md`. The k6 profile is in `load/k6-smoke.js` and is limited to five virtual users. Azure resources, pipeline runs, deployments, Selenium executions, and load-test results are not claimed until actually performed by an account owner.
