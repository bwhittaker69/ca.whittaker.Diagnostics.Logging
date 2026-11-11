# Contributing

Use the .NET 10 SDK with .NET 9 and .NET 10 runtimes. Tests use NUnit and Arrange-Act-Assert.

```powershell
dotnet build src/ca.whittaker.Diagnostics.Logging -c Release
dotnet test tests/ca.whittaker.Diagnostics.Logging.UnitTests -c Release
dotnet pack src/ca.whittaker.Diagnostics.Logging -c Release -o artifacts
dotnet restore samples/Logging.Sample --source "$PWD/artifacts"
dotnet run --project samples/Logging.Sample -c Release --no-restore
```

The sample references the prepared package, not the source project. Update its version when validating a different package or prerelease. Review package README/icon/licence/XML paths, dependencies, framework groups, repository commit, symbols, and documentation links before a release.

Open an issue to discuss a change, then submit a focused pull request with tests and documentation. Do not include credentials or real customer information. Contributions must be compatible with the repository's GPL-3.0-only licence.
