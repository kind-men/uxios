# Release Process

This document outlines the step-by-step procedure for updating the version, synchronizing files, and pushing changes to
GitHub. By following this process, new versions are seamlessly deployed to OpenUPM, ensuring they’re quickly available
to the community.

### Step 1: Update the Version String
- Update the `Uxios.Version` file with the new version string.

### Step 2: Synchronize `package.json`
- Ensure the `version` field in `package.json` matches the updated version string.

### Step 3: Update the Changelog
- Replace the `[Unreleased]` placeholder in the changelog with the new version string.
- Append the release date in `YYYY-mm-dd` format to the same line.

### Step 4: Push Changes
- Commit all changes and push them to the `main` branch on GitHub.

### Step 5: Create a Git Tag
- Create a tag for the new version, using the format `v[version]` (e.g., `v1.2.3`).

### Step 6: Verify OpenUPM Pipeline
- Once the tag is created, OpenUPM's build pipeline will automatically run.
- Verify the pipeline status [here](https://openupm.com/packages/com.kind-men.uxios/?subPage=pipelines).

Upon completion, the new version will be available on OpenUPM.
