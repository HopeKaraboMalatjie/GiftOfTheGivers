# Azure Pipelines: Part 3 CI/CD

`azure-pipelines.yml` runs on pushes to `master` using a Microsoft-hosted Windows agent. The CI steps restore and build the .NET 8 solution, run xUnit unit/integration tests, publish TRX results, generate an idempotent EF Core SQL migration script, package the web and Functions applications, and publish build artifacts. Selenium scenarios are skipped unless a test URL is configured; the donation-writing scenario also requires an explicit opt-in.

## Create and run the pipeline

1. In Azure DevOps, select **Pipelines > New pipeline**, choose this Azure Repos repository, and select the existing `/azure-pipelines.yml` file.
2. Ensure the pipeline is authorized to use the repository and any Azure Artifacts feeds. `NuGetAuthenticate@1` runs before package restore/tool installation.
3. Run the pipeline and inspect restore, build, test, migration-script generation, package, and artifact tasks. Test output is published as TRX results.
4. The `azureServiceConnection` parameter defaults to empty, so deployment tasks are omitted and CI can run without an Azure endpoint. Configure a test/staging target before enabling deployment; do not point test writes or load tests at production.

## Configure continuous deployment securely

Create an Azure Resource Manager service connection with least-privilege access to the target resources and authorize it for this pipeline. Azure DevOps service-connection inputs must be resolved at compile time, so use the `azureServiceConnection` YAML parameter rather than a runtime variable. Until the connection exists, leave its default empty; the deployment steps will be omitted. To deploy, enter the exact service-connection name as the parameter when manually running the pipeline, or set the YAML parameter default to that name to enable automatic deployment on `master`.

Define the remaining values as pipeline variables or place them in an authorized variable group:

| Variable | Value |
|---|---|
| `WebAppName` | Target Linux App Service name. The existing publish profile references `GiftOfTheGivers20260828193027`; verify that it is the intended target before using it. |
| `FunctionsAppName` | Target Linux Function App name, configured for .NET 8 isolated worker. |
| `SqlServerName` | Azure SQL server host name, for example `server-name.database.windows.net`. |
| `SqlDatabaseName` | Target Azure SQL database name. |
| `SqlUsername` | SQL deployment account name with only the required migration permissions. |
| `SqlPassword` | SQL deployment password; mark this variable **secret**. |

The SQL task runs the generated idempotent migration script before the web and Functions deployment tasks. Configure the App Service settings separately: `ConnectionStrings__DefaultConnection` (secret) and `AzureFunctions__BaseUrl`. Configure the Function App with `FUNCTIONS_WORKER_RUNTIME=dotnet-isolated`. Never put credentials in YAML, appsettings, commits, screenshots, or build logs. Rotate any credential that was previously committed; removing it from the current file does not erase it from Git history.

The deployment tasks are configured for Linux App Service and Linux Function App. Confirm target OS/runtime compatibility, SQL firewall/network access, and deployment permissions before enabling them. The application also checks/applies EF migrations at startup; pipeline SQL application is ordered before the app deployment to avoid first-start schema changes.

## Test and load-test execution

- Run unit and HTTP integration tests with `dotnet test GiftOfTheGivers.Tests/GiftOfTheGivers.Tests.csproj`.
- To enable the Selenium login-page test, set `GOTG_TEST_BASE_URL` to an isolated test site. For the donation-writing test, also set `GOTG_TEST_ALLOW_DONATION_WRITE=true`. Use a disposable database/site and Chrome; set `GOTG_UI_SCREENSHOT_DIR` to save PNG evidence.
- Run the five-user load profile with `k6 run -e BASE_URL=https://your-test-site.example load/k6-smoke.js`. The profile performs read-only GET requests to the donation form for 30 seconds. Save actual k6 output; do not claim a result until it has run.
- The application has only its existing initial EF migration. Part 3 adds migration deployment automation, not a new business-schema migration. Add and review a new EF migration when the data model changes.

## Azure Artifacts

For helper-package publication, create an Azure Artifacts feed and define `AzureArtifactsFeedUrl` as its NuGet v3 endpoint. When non-empty, the final pipeline step publishes the helper package; otherwise the package artifact is produced without feed publication.

Pipeline configuration is prepared in the repository; an Azure DevOps run, live deployment, live-site response, database update, and screenshots are not claimed until actually performed. See `PART3-EVIDENCE-CHECKLIST.md` for evidence capture.
