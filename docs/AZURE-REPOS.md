# Azure Repos workflow

These steps describe a reproducible workflow; no Azure DevOps repository is claimed as created by this project.

1. Create an Azure DevOps organization/project and an empty Azure Repos Git repository.
2. In Visual Studio, use **Git > Clone Repository** and authenticate to Azure DevOps.
3. Ensure the default branch is `main`.
4. Create `donations-feature` from `main`:
   ```powershell
   git switch -c donations-feature
   ```
   Commit the helper library, donation Function, and MVC donation integration, then push:
   ```powershell
   git add .
   git commit -m "Add donation certificate Function integration"
   git push -u origin donations-feature
   ```
5. Create `volunteer-feature` from updated `main`:
   ```powershell
   git switch main
   git pull
   git switch -c volunteer-feature
   ```
   Commit a meaningful volunteer workflow change (for example, volunteer validation/notification enhancement), then push:
   ```powershell
   git add .
   git commit -m "Improve volunteer workflow"
   git push -u origin volunteer-feature
   ```
6. Create pull requests for both feature branches in Azure Repos, review the changes, and merge them into `main`.
7. In Visual Studio or PowerShell verify the merge history:
   ```powershell
   git switch main
   git pull
   git log --graph --oneline --decorate --all
   ```

Do not commit passwords, local settings, package output, `bin`, `obj`, `.vs`, or access tokens. Use pipeline variables, service connections, and environment variables for secrets.
