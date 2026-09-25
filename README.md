# Gift of the Givers Foundation

.NET 8 ASP.NET Core MVC/Razor Pages prototype for relief projects, donations, volunteers, and employee updates.

## Solution projects

- `GiftOfTheGivers` - existing web application.
- `GiftOfTheGivers.Functions` - .NET 8 Azure Functions v4 isolated-worker HTTP endpoints.
- `GiftOfTheGivers.Helpers` - reusable donation/certificate helper library and NuGet package.
- `GiftOfTheGivers.Tests` - xUnit tests for helper business logic.

## Local development

1. Open `GiftOfTheGivers.sln` in Visual Studio.
2. Restore and build the solution.
3. Run the web project with the existing Development connection string and EF migrations.
4. To use the Function integration, run `GiftOfTheGivers.Functions` with Azure Functions Core Tools on port 7071. The web app defaults to `http://localhost:7071` in Development and falls back to its existing local certificate generator if the Function is unavailable.
5. See `PART2-SETUP.md` for complete local and Azure setup instructions.

No Azure deployment, Azure Repos repository, pipeline execution, or Azure Artifacts publication is claimed by this repository until performed by an account owner.
