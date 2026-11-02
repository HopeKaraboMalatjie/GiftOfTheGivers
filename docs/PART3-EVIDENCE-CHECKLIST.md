# Part 3 Evidence Checklist

Capture genuine screenshots/logs from the actual run. Redact credentials, tokens, donor/volunteer personal information, and subscription secrets. Record the date, branch, environment, and run ID. Do not fabricate screenshots or report skipped tests as passed.

## A. Application testing

- Visual Studio Test Explorer showing passing unit tests and the integration test.
- Test output/TRX showing test names, totals, and skipped Selenium tests where applicable.
- Test source demonstrating at least three meaningful unit tests and the donation form-to-database integration flow.
- Selenium code and successful output for at least two scenarios against an isolated test site; include the test URL/environment and screenshots.
- k6 command/configuration showing five virtual users, plus the real summary/results (request count, failure rate, p95 response time).

## B. Azure Pipelines

- `azure-pipelines.yml` showing the `master` trigger, automated test run, test-result publication, and deployment tasks.
- Azure DevOps run triggered by a commit, including restore/build/test logs and published test results.
- Web, Functions, migration-script, and load-test artifacts from that run.
- Screenshots of deployment task results only after an actual Azure deployment.

## C. Continuous deployment and SQL

- Azure service connection and secured variable/variable-group names (never show secret values).
- Azure App Service Overview page with deployed URL, runtime, and successful live-site page.
- Azure Function App Overview and a real GET health response plus tested POST response.
- Azure SQL deployment task result and evidence of the migration history/schema after applying the generated idempotent script.
- Record whether any new schema migration was added for Part 3. Do not describe the existing initial migration as a new Part 3 schema change.

## D. Handover

- Completed `docs/USER-MANUAL.md` with real application screenshots and simple captions.
- Completed one-page `docs/TEAM-CONTRIBUTION-REPORT.md`, including actual member roles and commit/pipeline evidence.
- Record any unavailable Azure resource, skipped test, or outstanding limitation honestly.
