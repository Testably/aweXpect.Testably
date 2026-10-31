# Workflows

Commands run from the repository root. `build.ps1` (Windows), `build.sh` (Linux, macOS) and `build.cmd` compile
`Pipeline/Build.csproj` and run one of its Fallout build targets; `.fallout/` holds the build parameters.

## Build

```powershell
dotnet build aweXpect.Testably.slnx
```

Builds the library for all target frameworks (and packs it, as `GeneratePackageOnBuild` is set), the tests, the AOT
smoke tests and the benchmarks in `Debug`. The pipeline builds `Debug` locally and `Release` on CI.

## Tests

```powershell
dotnet test --project Tests/aweXpect.Testably.Tests
dotnet test --project Tests/aweXpect.Testably.Tests --no-build -f net10.0 --treenode-filter "/*/*/FileInfo+HasLength+*/*"
dotnet test --project Tests/aweXpect.Testably.Tests --no-build -f net48
```

- The tests use [TUnit](https://tunit.dev) and target `net8.0`, `net10.0` and `net48`. `net48` runs only on Windows.
- All test projects are executables of the Microsoft.Testing.Platform, which `global.json` selects for `dotnet test`,
  so the project is passed with `--project` and a filter with `--treenode-filter`
  (`/<assembly>/<namespace>/<class>/<test>`). `dotnet run --project Tests/aweXpect.Testably.Tests -f net10.0 -- <options>`
  starts the same executable.
- Test classes are nested (`FileInfo` > `HasLength` > `Tests`), so the class name in a filter uses `+`.
- `./build.ps1 UnitTests` runs every framework the way CI does (`net48` on Windows only), each writing
  `<project>_<framework>.trx` and `<project>_<framework>.cobertura.xml` to `TestResults`. `./build.ps1 CodeCoverage`
  adds the SonarQube coverage report in `TestResults/reports`.

## Public API

The public API of every target framework of the library is stored as a snapshot in `Tests/aweXpect.Testably.Api.Tests`
(`ApiApprovalTests.aweXpect%2ETestably.<framework>.awexpect-snapshot.txt`), compared with
`aweXpect.Snapshots.PublicApi`.

```powershell
dotnet test --project Tests/aweXpect.Testably.Api.Tests
./build.ps1 ApiChecks
```

- The tests read `Source/aweXpect.Testably/bin/<Configuration>/<framework>/aweXpect.Testably.dll` for each framework in
  `Source/Directory.Build.props`, so they test the build of the same configuration. Building the API test project builds
  all library frameworks.
- When the API changes, the test fails and writes `*.awexpect-snapshot.pending.txt`. To accept an intended change,
  either rename the pending file to `*.awexpect-snapshot.txt`, run the explicit test `AcceptApiChanges`
  (`dotnet test --project Tests/aweXpect.Testably.Api.Tests --treenode-filter "/*/*/ApiApprovalTests/AcceptApiChanges"`),
  or run the tests with `AWEXPECT_ACCEPT_SNAPSHOTS=all`. Commit the snapshots, never the pending files.
- `AcceptApiChanges` is `[Explicit]`: TUnit runs it only when every test that the filter selects is explicit, so a
  normal run and CI skip it.

## AOT smoke tests

```powershell
./build.ps1 AotSmokeTests
dotnet run --project Tests/Aot/aweXpect.Testably.Aot
```

`Tests/Aot/aweXpect.Testably.Aot` is a console app (no test framework) that runs the checks in `Checks.cs` and returns
a non-zero exit code on a failure. The pipeline target runs it from the build output, then publishes it with
`PublishAot` for the current runtime into `Artifacts/Aot` and runs the native executable. Trimmer and ILC warnings are
errors. The publish needs the Native AOT prerequisites of the platform (on Windows, the C++ build tools).
`dotnet run` alone only covers the JIT half.

## Benchmarks

```powershell
./build.ps1 Benchmarks
dotnet run -c Release --project Benchmarks/aweXpect.Testably.Benchmarks -- --filter *
```

BenchmarkDotNet benchmarks in `Benchmarks/aweXpect.Testably.Benchmarks` (`HappyCaseBenchmarks`, split into partial
files per expectation; list them with `-- --list flat`). The pipeline target writes the results to
`Artifacts/Benchmarks`, and on a PR `BenchmarkComment` posts them as a comment.

## Mutation tests

```powershell
./build.ps1 MutationTests
```

Runs [Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/) on `Source/aweXpect.Testably` against
`Tests/aweXpect.Testably.Tests` and `Tests/aweXpect.Testably.Api.Tests` (net10.0, mutation level `Advanced`) with
Stryker's runner for the Microsoft.Testing.Platform (`--test-runner mtp`) and writes reports to `Artifacts/Stryker`.
The Stryker version is the `PackageDownload` of `dotnet-stryker` in `Pipeline/Build.csproj`. Off `main` only code
changed since `main` is mutated. CI runs it on `windows-latest`; it takes long, so do not run it locally unless asked.
`MutationTestsDashboard` uploads the report to the Stryker dashboard and `MutationTestsComment` comments on the PR.

## Documentation

- `README.md` is the package readme. `Pack` and `UpdateReadme` write a release copy with versioned badges to
  `Artifacts/README.md`. `Docs/pages/00-index.md` is the docs landing page and is maintained separately; keep its
  overview tables in sync with the README.
- `Docs/pages/*.md` is the user documentation, published to the Testably docs site: after a successful `Build` on
  `main`, `notify-docs-site.yml` triggers a rebuild of `Testably/Testably.Site`. The code blocks are not compiled; the
  README examples are covered by `Tests/aweXpect.Testably.Tests/Readme.Tests.cs`.
- `Docs/agents/` holds these guides and is not published.

## Pipeline targets

`Pipeline/Build*.cs` defines the targets (list them with `./build.ps1 --help`):

| Target                  | Purpose                                                                          |
|-------------------------|----------------------------------------------------------------------------------|
| `Compile`               | Clean `Artifacts/` and `TestResults/`, restore, compute the version, build       |
| `UnitTests`             | .NET and net48 unit tests with coverage                                          |
| `CodeCoverage`          | `UnitTests` plus the coverage report                                             |
| `ApiChecks`             | Public API snapshot tests                                                        |
| `AotSmokeTests`         | AOT smoke tests under the JIT and as a Native AOT publish                        |
| `Benchmarks`            | Run the benchmarks and log the report                                            |
| `BenchmarkComment`      | Post the benchmark results to the PR (CI only)                                   |
| `MutationTests`         | Stryker run and PR comment body                                                  |
| `MutationTestsDashboard` | Upload the mutation report to the Stryker dashboard (CI only)                    |
| `MutationTestsComment`  | Post the mutation results to the PR (CI only)                                    |
| `CodeAnalysis`          | SonarCloud analysis around `Compile` and `CodeCoverage` (needs a token)          |
| `Pack`                  | Build and collect the `.nupkg` / `.snupkg` into `Artifacts/Packages`             |

Running `./build.ps1` without a target runs `ApiChecks`, `Benchmarks` and `CodeAnalysis`. Versions come from
GitVersion (`-DEV` locally, `-CI` on CI, the tag on a release).

## CI workflows

- `ci.yml` (pull requests to `main`): unit tests on Ubuntu, Windows and macOS (`CodeCoverage`), AOT smoke tests on all
  three, API tests, mutation tests (Windows), benchmarks, SonarCloud analysis (not for forks or Dependabot) and a test
  result summary.
- `ci-analysis.yml` (after `CI`): uploads the mutation report and posts the mutation and benchmark comments.
- `build.yml` (push to `main`, tags `v*`): the same jobs, plus `Pack`; on a tag, `Push` publishes to nuget.org once the
  test jobs, API tests and analysis succeeded, creates the GitHub release and comments on the released issues.
- `notify-docs-site.yml`: triggers the docs site rebuild.
- Dependabot updates actions and NuGet packages weekly (prefix `chore`); Mergify squash-merges its PRs once the unit
  tests and API tests pass.
