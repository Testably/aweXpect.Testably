# Agent guide

aweXpect.Testably adds [aweXpect](https://github.com/Testably/aweXpect) expectations for the file system and time
system mocks of [Testably.Abstractions](https://github.com/Testably/Testably.Abstractions). It ships one NuGet package,
`aweXpect.Testably`, built from `Source/aweXpect.Testably`.

## SDK and build

- Use the SDK pinned in `global.json` (`latestPatch` roll forward). The tests also need the .NET 8 runtime, and the
  net48 leg runs only on Windows.
- Package versions live in `Directory.Packages.props` (central package management); never put a version in a project
  file. `nuget.config` allows nuget.org only.
- Library settings live in `Source/Directory.Build.props`: warnings are errors (only CS1591 and NU5104 stay warnings),
  nullable is enabled (annotations only on netstandard2.0), XML docs and the package are generated on build. Assemblies
  are strong-named with `strongname.snk`; test projects are not signed.
- Build with `dotnet build aweXpect.Testably.slnx`. `Pipeline/Build.csproj` is in the solution but excluded from its
  build; it runs through `build.ps1` / `build.sh`.

## Design goals

Correctness is a must. The other goals are strong defaults and win in this order when they conflict: stable and compatible, AOT and trimming friendly, a consistent and pleasant API, a fast happy path, extensible, errors at compile time, documented. [Docs/agents/design-goals.md](Docs/agents/design-goals.md) explains each goal, the code rules, the checks before a commit and the goals of this repository.

## Project constraints

- Target frameworks: the library builds for `netstandard2.0;net8.0;net10.0`, the tests for `net8.0;net10.0;net48`.
  Library code must compile on netstandard2.0 (polyfills in `Source/aweXpect.Testably/Polyfills`, newer APIs behind
  `#if NET8_0_OR_GREATER`). Test code must compile on net48: no records and no list patterns.
- Dependency floors: the library references only `aweXpect.Core` and `Testably.Abstractions.Testing`; their versions in
  `Directory.Packages.props` are the minimum every consumer gets. Raise them only on purpose, and do not add library
  dependencies. `aweXpect` and `Testably.Abstractions` are test-only.
- AOT and trimming: net8.0+ builds set `IsTrimmable` and `IsAotCompatible`, so trim and AOT analyzer warnings fail the
  build. Avoid reflection; annotate it where it cannot be avoided. Behaviour under Native AOT is checked by
  `Tests/Aot/aweXpect.Testably.Aot`; add a check to its `Checks.cs` when an expectation adds a new kind of code path.
- Public API: everything public lives in the `aweXpect.Testably` namespace (statistics mirrors in
  `aweXpect.Testably.Recorded`). Each public expectation has XML docs and `[GuaranteesNotNull]`, validates arguments
  with `ThrowHelper.ThrowIfNull` and calls user delegates through `UserCode.Invoke`. Any public API change shows up in
  the snapshots of `Tests/aweXpect.Testably.Api.Tests` and is reviewed with the PR.
- Failure messages follow the aweXpect conventions; tests pin the complete message with `.WithMessage(...)`.

## Validation

- All tests run on TUnit and the Microsoft.Testing.Platform, which `global.json` selects for `dotnet test`: pass the
  project with `--project` and filter with `--treenode-filter`.
- Run the tests that cover your change before pushing: `dotnet test --project Tests/aweXpect.Testably.Tests` (add
  `-f net10.0` and a `--treenode-filter` for a quick loop, but run net48 too when you touch language features).
- Public API: `dotnet test --project Tests/aweXpect.Testably.Api.Tests`. For an intended change, accept the snapshots
  (see [workflows](Docs/agents/workflows.md#public-api)) and commit the updated `*.awexpect-snapshot.txt` files, never
  a `*.awexpect-snapshot.pending.txt`.
- AOT: `./build.ps1 AotSmokeTests` runs the smoke tests under the JIT and as a Native AOT publish.
- `Readme.Tests.cs` mirrors the examples in `README.md`; change both together. `Docs/pages/` is published to the
  docs site, so keep it in sync with the API.
- Timing-dependent tests (watchers, notifications, timers) must stay robust on slow CI runners.
- Never run `./build.ps1` without a target: the defaults are `ApiChecks`, `Benchmarks` and `CodeAnalysis` (SonarCloud).
- Do not run mutation tests or benchmarks locally unless asked; CI runs them on every PR.

## File conventions

- `.editorconfig` asks for tabs and CRLF; git stores LF (`* text=auto`). Keep line endings consistent inside a file,
  as expected failure messages are raw string literals. Prefer an editing tool over `sed -i` or PowerShell rewrites.
- Some `.cs` files have a UTF-8 BOM and most do not. Keep each file's encoding: PowerShell `Set-Content` / `Out-File`
  add or strip a BOM, so check `git diff` for a U+FEFF change. New files are UTF-8 without BOM.
- Do not add em-dashes to docs, the README or XML docs.

## Git workflow

- Work on a feature branch and open a PR against `main`; PRs are squash-merged and their titles follow
  [conventional commits](https://www.conventionalcommits.org/) (`feat:`, `fix:`, `docs:`, `test:`, `refactor:`,
  `ci:`, `chore:`).
- Commit message: the subject is the PR title, the body explains why (the diff shows what), each paragraph on a single
  line, no co-author or attribution trailers.
- Never commit `.claude/`, `Artifacts/` or `TestResults/`.
- Releases are tags `vX.Y.Z` on `main`, which make the `Build` workflow push to nuget.org; do not create tags.

## References

- [Docs/agents/design-goals.md](Docs/agents/design-goals.md): the goals and code rules for every change, the checks before a commit and the goals of this repository.
- [Docs/agents/workflows.md](Docs/agents/workflows.md): build, test, API, AOT, benchmark and mutation commands,
  pipeline targets and CI workflows.
- [Docs/agents/project-structure.md](Docs/agents/project-structure.md): repository layout, source organisation and
  test structure.
- [CONTRIBUTING.md](CONTRIBUTING.md): contributor expectations (SonarCloud quality gate, coverage, Stryker).
