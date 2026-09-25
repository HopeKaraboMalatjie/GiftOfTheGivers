# Part 2 evidence checklist

Capture real screenshots and include the date/context where useful. Do not claim evidence for steps not performed.

## Section A - Azure Functions

- Solution Explorer showing `GiftOfTheGivers.Functions` in the existing solution.
- `DonationCertificateFunction.cs` showing HTTP trigger, validation, logging, and PDF generation.
- `EmployeeUpdateFunction.cs` showing validation and ILogger usage.
- Azure Functions Core Tools terminal showing the local host running.
- Browser GET request to `/api/donationcertificate` showing ready status.
- Postman POST request/response for donation and the downloaded demonstration PDF showing the disclaimer.
- Postman POST request/response for employee update and the local Function log.
- MVC donation certificate download while the local Function is running.
- Azure portal Function App deployment/status only after an actual deployment.

## Section B - Azure Repos

- Azure DevOps project and repository page.
- `main`, `donations-feature`, and `volunteer-feature` branches.
- Meaningful commits on each feature branch.
- Pull request review and merge screen.
- `main` commit history showing merged feature work.

## Section C - Azure Pipelines

- `azure-pipelines.yml` in the repository.
- Pipeline creation showing Azure Repos YAML selection.
- A real pipeline run showing restore and build success.
- Test task and published xUnit results.
- Web, Functions, and helper-package artifacts.
- Commit to `main` and the resulting automatic pipeline run.

## Section D - Azure Artifacts

- `GiftOfTheGivers.Helpers` project in Solution Explorer.
- `DonationHelper.cs` methods and MVC usage in `DonationController`.
- Output of `dotnet pack` containing the generated nupkg (do not commit it).
- Azure Artifacts feed page after creating the feed.
- Published `GiftOfTheGivers.Helpers` package page.
- Authenticated package source/NuGet configuration without exposing credentials.
- MVC project consuming a published PackageReference, if that transition is performed.
