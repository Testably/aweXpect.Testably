# Project structure

## Repository layout

| Path                                       | Content                                                                  |
|--------------------------------------------|--------------------------------------------------------------------------|
| `Source/aweXpect.Testably`                 | The library and only shipped package                                     |
| `Tests/aweXpect.Testably.Tests`            | Unit tests (xunit v2, `net8.0;net10.0;net48`)                            |
| `Tests/aweXpect.Testably.Api.Tests`        | Public API snapshots (NUnit, `net10.0`)                                  |
| `Tests/Aot/aweXpect.Testably.Aot`          | Native AOT smoke tests (console app, `net10.0`)                          |
| `Benchmarks/aweXpect.Testably.Benchmarks`  | BenchmarkDotNet benchmarks                                               |
| `Pipeline`                                 | Fallout build targets used by `build.ps1` / `build.sh` and CI            |
| `Docs/pages`                               | User documentation, published to the Testably docs site                  |
| `Docs/agents`                              | Guides for authors and agents, not published                             |
| `.github`                                  | CI workflows, Dependabot and Mergify configuration                       |

Shared MSBuild settings: `Directory.Build.props` (strong naming), `Source/Directory.Build.props` (package metadata,
target frameworks, warnings, trimming and AOT flags), `Tests/Directory.Build.props` (test frameworks and packages) and
`Tests/Aot/Directory.Build.props` (AOT publish settings). `Pipeline` has its own `Directory.Build.*` files that stop the
root ones from applying.

## Library (`Source/aweXpect.Testably`)

- Expectations are extension methods on `IThat<T>`, grouped by subject in `public static partial class
  <Subject>Extensions`, with one file per expectation and its negation: `<Subject>Extensions.<Expectation>.cs`
  (for example `FileInfoExtensions.HasLength.cs` holds `HasLength` and its constraint). The subjects are
  `IFileSystem`, `IFileInfo`, `IDirectoryInfo`, `IDriveInfo`, `IFileVersionInfo`, `ChangeDescription` (notifications),
  `IFileSystemWatcher`, `ITimerMock` and `IFileSystemStatistics`.
- A constraint is usually a private nested class deriving from `ConstraintResult.WithNotNullValue<T>` (aweXpect.Core)
  that sets the `Outcome` and writes the normal and negated expectation and result texts. A subject that does not
  exist fails both ways (`Outcome.FailureBothWays`).
- `Helpers/`: internal constraints shared between subjects (`FileSystemConstraints`, `FileVersionInfoConstraints`,
  `NotificationConstraints`, `StatisticsConstraints`, `TimerConstraints`), `ParameterMatcher` for statistics
  predicates, `ThrowHelper` for argument validation and `Constants`.
- `Results/`: result types returned by expectations with follow-ups, such as `FileResult` (`.WithContent(...)`,
  `.Which`), `DirectoryResult`, `DriveResult`, the triggered and not-triggered notification and watcher results, and
  the recorded call results.
- `Recorded/`: the `aweXpect.Testably.Recorded` namespace. `StatisticsExtensions.Recorded()` returns
  `RecordedFileSystemStatistics`, a mirror of `IFileSystemStatistics` whose methods and properties
  (`.File.WriteAllText()`, `.FileInfo["foo.txt"].IsReadOnly.Set()`) match the recorded calls by position. The remarks
  on `Recorded()` describe the matching rules.
- `Polyfills/`: attributes missing on netstandard2.0, compiled only below net8.0.
- `Usings.cs`: global usings for `aweXpect.Core.Extending` and `aweXpect.Formatting`.

## Tests (`Tests/aweXpect.Testably.Tests`)

- The tests mirror the subjects: a (usually partial) class per subject (`FileInfo`, `DirectoryInfo`, `FileSystem`, `DriveInfo`,
  `FileVersionInfo`, `ChangeDescriptionTests`, `FileSystemWatcher`, `Statistics`, `Timer`), a nested class per
  expectation and a nested `Tests` class, in files named `<Subject>.<Expectation>.Tests.cs`. Shared helpers of a
  subject sit in `<Subject>.cs`.
- Cross-cutting tests that a new expectation usually needs an entry in: `ArgumentValidation.Tests.cs` (null
  arguments), `NullOrMissingSubject.Tests.cs` (null and missing subjects, including negation), `PluralGrammar.Tests.cs`
  (messages inside `ThatAll`) and `Quantifiers.Tests.cs` (counts such as `Once()` or `AtLeast(...)` on recorded calls,
  notifications, watchers and timers).
- `Readme.Tests.cs` runs the examples of `README.md`.
- `TestTimes.cs` provides a random fixed time per run, so that time-based tests do not rely on special times.
- The tests use `MockFileSystem` and `MockTimeSystem` from Testably.Abstractions.Testing and assert failure messages
  with raw string literals.

## Other projects

- `Tests/aweXpect.Testably.Api.Tests`: `ApiApprovalTests` verifies one snapshot per library target framework.
  `Helper` finds the solution directory by `aweXpect.Testably.slnx` and reads the frameworks from
  `Source/Directory.Build.props`.
- `Tests/Aot/aweXpect.Testably.Aot`: `Program` runs every `Check` in `Checks.All`; each check states a passing or a
  failing expectation, with the expected message parts for the failing ones.
- `Pipeline`: one partial `Build` class per area (`Build.Compile.cs`, `Build.UnitTest.cs`, `Build.ApiChecks.cs`,
  `Build.AotTest.cs`, `Build.Benchmarks.cs`, `Build.MutationTests.cs`, `Build.CodeAnalysis.cs`,
  `Build.CodeCoverage.cs`, `Build.Pack.cs`); see [workflows](workflows.md#pipeline-targets).
