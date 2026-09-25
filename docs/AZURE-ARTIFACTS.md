# Azure Artifacts and the helper package

`GiftOfTheGivers.Helpers` is a real .NET 8 class library with NuGet metadata and produces `GiftOfTheGivers.Helpers.nupkg` using `dotnet pack`. The MVC application uses a `ProjectReference` during local development so no private feed is required to compile.

## Publish

1. Create an Azure DevOps project and an Azure Artifacts feed.
2. Copy `NuGet.config.template` to a private local `NuGet.config` and replace organization, project, and feed placeholders. Do not commit the private file if it contains credentials.
3. Authenticate using the Azure Artifacts Credential Provider or a secure Azure DevOps service connection.
4. Pack the helper library:
   ```powershell
   dotnet pack GiftOfTheGivers.Helpers/GiftOfTheGivers.Helpers.csproj --configuration Release
   ```
5. Push the package using the feed endpoint and authenticated credentials, or configure `AzureArtifactsFeedUrl` in the pipeline.

## Consume from the feed

After the package is published and feed authentication is available, replace the MVC project's local `ProjectReference` with a `PackageReference` for `GiftOfTheGivers.Helpers` at the published version. Keep the ProjectReference for local/offline development until the feed is configured. The application must not require Azure Artifacts merely to build on a new developer machine.

No feed or package publication is claimed as completed without Azure account access.
