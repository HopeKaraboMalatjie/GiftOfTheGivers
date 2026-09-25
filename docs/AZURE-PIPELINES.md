# Azure Pipelines

`azure-pipelines.yml` triggers on pushes to `main` and uses a Microsoft-hosted Windows agent.

## Create the pipeline

1. Push this repository to Azure Repos.
2. In Azure DevOps choose **Pipelines > New pipeline**.
3. Select **Azure Repos Git**, select the repository, and choose **Existing Azure Pipelines YAML file**.
4. Select `/azure-pipelines.yml`, review it, and run the pipeline.
5. The pipeline installs .NET 8, restores the solution, builds it, runs the xUnit tests, publishes the MVC project and Functions project, packs the helper library, and publishes three build artifacts.
6. Inspect the Restore, Build, Test, Publish, and Artifact tasks in the run summary. Test results are published as TRX results.

## Azure service connections and artifacts

For deployment, add an Azure Resource Manager service connection and add deployment tasks after the published artifacts. Do not put credentials in YAML. For package publication, create an Azure Artifacts feed and define the secret/authorized variable `AzureArtifactsFeedUrl` as the feed's NuGet v3 endpoint. When that variable is non-empty, the final pipeline step publishes the helper package; otherwise package publication is skipped while the package artifact is still produced.

The pipeline has not been executed against an Azure DevOps account by this repository, so a live successful run is not claimed.
