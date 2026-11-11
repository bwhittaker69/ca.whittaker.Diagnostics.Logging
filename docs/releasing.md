# Releasing

The manual `publish-nuget.yml` workflow runs only on `main`. The `nuget` environment restricts deployments to that branch. Ordinary pushes and pull requests never trigger publication.

Use `mode: authenticate` to test the NuGet Trusted Publishing policy without building or publishing packages. Its job contains no package upload step. A successful token exchange confirms authentication; permanent policy activation must be checked separately in the NuGet account.

Use `mode: publish` after release review. The workflow builds and tests the solution in Release, packs it, inspects metadata/documentation/Source Link/symbols, and runs the sample against the package before requesting a short-lived credential. Only the authentication and publication jobs have `id-token: write`.

The official `NuGet/login` action is pinned to the verified v1 commit. Credentials are used only by the publishing step, never printed or uploaded. The only uploaded artifacts are packages and symbols. Dependencies use reviewed revision pins in this workflow.

Version changes require updating the project, README installation command, sample reference, changelog, and explicit artifact names in this workflow together. Do not overwrite a published version. Verify the NuGet listing, README, links, Source Link, symbols, and consumer restore after publication. After the first package is published, tighten its policy scope to push only new versions of this package.

If a policy's initial activation window expires, reactivate it in the NuGet account before retrying. No long-lived API key is required.
