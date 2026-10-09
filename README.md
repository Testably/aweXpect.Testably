# aweXpect.Testably

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)
[![Build](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Testably)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Testably)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Testably%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Testably/main)

Drop-in [aweXpect](https://github.com/aweXpect/aweXpect) expectations for the
file-system and time-system mocks from
[Testably.Abstractions](https://github.com/Testably/Testably.Abstractions):
`MockFileSystem`, `IFileInfo`, `IDirectoryInfo`, `IDriveInfo`,
`IFileVersionInfo`, `IFileSystemWatcher`, `IFileSystemStatistics` and
`ITimerMock`.

The samples use the `TimeSpan` helpers (`1.Second()`, `100.Milliseconds()`)
from [aweXpect.Chronology](https://github.com/Testably/aweXpect.Chronology).

## File system (`IFileSystem`)

Verify that a file, directory or drive is present in the file system. Every
positive assertion has a `DoesNot…` counterpart:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("beatles/abbey-road");
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await That(fileSystem).HasDirectory("beatles/abbey-road");
await That(fileSystem).HasFile("let-it-be.txt");

await That(fileSystem).DoesNotHaveDirectory("beatles/white-album");
await That(fileSystem).DoesNotHaveFile("yesterday.txt");
```

### File chain

`HasFile(path)` returns a result that lets you chain assertions about the
file's content and timestamps without re-resolving it:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await That(fileSystem).HasFile("let-it-be.txt").WithContent("LET IT BE").IgnoringCase();
await That(fileSystem).HasFile("let-it-be.txt").WithContent().NotEqualTo("let it go");
await That(fileSystem).HasFile("let-it-be.txt").WithContent(Encoding.UTF8.GetBytes("let it be"));
```

You can compare against another file on the same file system:

```csharp
fileSystem.File.WriteAllText("let-it-be-remastered.txt", "LET IT BE");
fileSystem.File.WriteAllText("hey-jude.txt", "hey jude");

await That(fileSystem).HasFile("let-it-be.txt").WithContent().SameAs("let-it-be-remastered.txt").IgnoringCase();
await That(fileSystem).HasFile("let-it-be.txt").WithContent().NotSameAs("hey-jude.txt");
```

…and against the file's timestamps. `.Within(tolerance)` widens the comparison
to a window:

```csharp
await That(fileSystem).HasFile("let-it-be.txt").WithCreationTime(DateTime.Now).Within(1.Second());
await That(fileSystem).HasFile("let-it-be.txt").WithLastAccessTime(DateTime.Now).Within(1.Second());
await That(fileSystem).HasFile("let-it-be.txt").WithLastWriteTime(DateTime.Now).Within(1.Second());
```

### Directory chain

`HasDirectory(path)` exposes sub-collections:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("beatles/abbey-road");
fileSystem.Directory.CreateDirectory("beatles/revolver");
fileSystem.File.WriteAllText("beatles/abbey-road/something.txt", "something");

await That(fileSystem).HasDirectory("beatles").WithDirectories(d => d.HasCount().EqualTo(2));
await That(fileSystem).HasDirectory("beatles/abbey-road").WithFiles(f => f
    .All().ComplyWith(x => x.HasContent("SOMETHING").IgnoringCase()));
```

### Bridging to `IFileInfo` / `IDirectoryInfo` / `IDriveInfo` via `.Which`

`HasFile`, `HasDirectory` and `HasDrive` each expose a `.Which` property that
returns the resolved `IFileInfo` / `IDirectoryInfo` / `IDriveInfo` so the
subject-level assertions below light up directly in the chain:

```csharp
await That(fileSystem).HasFile("let-it-be.txt").Which.HasLength(9).And.HasContent("let it be");
await That(fileSystem).HasDirectory("beatles/revolver").Which.IsEmpty();
await That(fileSystem).HasDrive("D:\\").Which.IsReady().And.HasDriveFormat("NTFS");
```

## File (`IFileInfo`)

```csharp
IFileInfo fileInfo = fileSystem.FileInfo.New("let-it-be.txt");

await That(fileInfo).Exists();
await That(fileInfo).DoesNotExist();

await That(fileInfo).HasName("let-it-be.txt");
await That(fileInfo).HasExtension(".txt");
await That(fileInfo).HasLength(9);
await That(fileInfo).HasContent("let it be");
await That(fileInfo).HasContent(Encoding.UTF8.GetBytes("let it be"));

await That(fileInfo).IsReadOnly();
await That(fileInfo).IsNotReadOnly();

await That(fileInfo).HasAttribute(FileAttributes.ReadOnly);
await That(fileInfo).DoesNotHaveAttribute(FileAttributes.Hidden);

await That(fileInfo).HasCreationTime(DateTime.Now).Within(1.Second());
await That(fileInfo).HasLastAccessTime(DateTime.Now).Within(1.Second());
await That(fileInfo).HasLastWriteTime(DateTime.Now).Within(1.Second());
```

`HasAttribute` / `DoesNotHaveAttribute` use flag containment, so
`FileAttributes.ReadOnly | FileAttributes.Hidden` satisfies
`HasAttribute(FileAttributes.ReadOnly)`. The empty / `default` value is
rejected with an `ArgumentException` to avoid silent passes.

On .NET 10 or later, `WhoseParent` switches the subject to the containing
directory so the directory-level assertions can be reused:

```csharp
await That(fileInfo).WhoseParent.HasName("beatles").And.IsNotEmpty();
```

## Directory (`IDirectoryInfo`)

```csharp
IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("beatles");

await That(dirInfo).Exists();
await That(dirInfo).DoesNotExist();

await That(dirInfo).HasName("beatles");

await That(dirInfo).IsEmpty();
await That(dirInfo).IsNotEmpty();

await That(dirInfo).HasFile("abbey-road/something.txt");
await That(dirInfo).DoesNotHaveFile("abbey-road/yesterday.txt");
await That(dirInfo).HasDirectory("abbey-road").Which.HasFile("something.txt");
await That(dirInfo).DoesNotHaveDirectory("white-album");

await That(dirInfo).HasAttribute(FileAttributes.Directory);
await That(dirInfo).DoesNotHaveAttribute(FileAttributes.Hidden);

await That(dirInfo).HasCreationTime(DateTime.Now).Within(1.Second());
await That(dirInfo).HasLastAccessTime(DateTime.Now).Within(1.Second());
await That(dirInfo).HasLastWriteTime(DateTime.Now).Within(1.Second());
```

`HasFile` / `HasDirectory` on `IDirectoryInfo` return the same chain
results as the file-system-level versions, so `.WithContent(...)`,
`.WithLastWriteTime(...)`, `.Which`, etc. all work as well: the path is
resolved relative to the directory.

On .NET 10 or later, `WhoseParent` switches to the parent directory:

```csharp
await That(dirInfo).WhoseParent.HasName("…");
```

## Drive (`IDriveInfo`)

```csharp
MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));

IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

await That(driveInfo).HasAvailableFreeSpace(2048);
await That(driveInfo).HasTotalSize(2048).And.HasTotalFreeSpace(2048);
await That(driveInfo).HasDriveFormat("NTFS");
await That(driveInfo).HasDriveType(DriveType.Fixed);
await That(driveInfo).HasName(driveInfo.Name).And.HasVolumeLabel(driveInfo.VolumeLabel);
await That(driveInfo).IsReady();
```

> Drives are matched by name (case-insensitive) against
> `IFileSystem.DriveInfo.GetDrives()`. UNC drives, which do not appear in
> `GetDrives()`, are not supported by `HasDrive`.

## File version info (`IFileVersionInfo`)

`IFileVersionInfo` instances obtained via
`MockFileSystem.FileVersionInfo.GetVersionInfo` can be asserted directly. The
values come from `MockFileSystem.WithFileVersionInfo(glob, builder)`:

```csharp
MockFileSystem fileSystem = new();
fileSystem.WithFileVersionInfo("*.dll", v => v
    .SetCompanyName("Apple Corps")
    .SetProductName("Abbey Road")
    .SetFileVersion("1.9.6.9")
    .SetIsDebug(true));
fileSystem.File.WriteAllText("AbbeyRoad.dll", "");

IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("AbbeyRoad.dll");

await That(info).HasCompanyName("Apple Corps").And.HasProductName("Abbey Road");
await That(info).HasFileVersion("1.9.6.9").And.HasFileMajorPart(1);
await That(info).IsDebug().And.IsNotPreRelease();
```

Dedicated assertions cover every `IFileVersionInfo` property — strings via
`Has…(string)`, the integer version parts via `Has…(int)` and the booleans
as `Is…()` / `IsNot…()` pairs.

| Property             | Assertion                                  |
|----------------------|--------------------------------------------|
| `Comments`           | `HasComments(string)`                      |
| `CompanyName`        | `HasCompanyName(string)`                   |
| `FileDescription`    | `HasFileDescription(string)`               |
| `FileName`           | `HasFileName(string)`                      |
| `FileVersion`        | `HasFileVersion(string)`                   |
| `InternalName`       | `HasInternalName(string)`                  |
| `Language`           | `HasLanguage(string)`                      |
| `LegalCopyright`     | `HasLegalCopyright(string)`                |
| `LegalTrademarks`    | `HasLegalTrademarks(string)`               |
| `OriginalFilename`   | `HasOriginalFilename(string)`              |
| `PrivateBuild`       | `HasPrivateBuild(string)`                  |
| `ProductName`        | `HasProductName(string)`                   |
| `ProductVersion`     | `HasProductVersion(string)`                |
| `SpecialBuild`       | `HasSpecialBuild(string)`                  |
| `FileBuildPart`      | `HasFileBuildPart(int)`                    |
| `FileMajorPart`      | `HasFileMajorPart(int)`                    |
| `FileMinorPart`      | `HasFileMinorPart(int)`                    |
| `FilePrivatePart`    | `HasFilePrivatePart(int)`                  |
| `ProductBuildPart`   | `HasProductBuildPart(int)`                 |
| `ProductMajorPart`   | `HasProductMajorPart(int)`                 |
| `ProductMinorPart`   | `HasProductMinorPart(int)`                 |
| `ProductPrivatePart` | `HasProductPrivatePart(int)`               |
| `IsDebug`            | `IsDebug()` / `IsNotDebug()`               |
| `IsPatched`          | `IsPatched()` / `IsNotPatched()`           |
| `IsPreRelease`       | `IsPreRelease()` / `IsNotPreRelease()`     |
| `IsPrivateBuild`     | `IsPrivateBuild()` / `IsNotPrivateBuild()` |
| `IsSpecialBuild`     | `IsSpecialBuild()` / `IsNotSpecialBuild()` |

## File-system notifications

A `MockFileSystem` raises notifications when files or directories change. Run
the code under test, then assert against the notifications it produced:

```csharp
MockFileSystem fileSystem = new();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await That(fileSystem).TriggeredNotification();
await That(fileSystem).TriggeredNotification(c => c.Name == "let-it-be.txt");
```

`.Within(timeout)` (default 30 s) lets the assertion wait for asynchronous
notifications. If a matching notification already fired, the assertion
completes synchronously; otherwise it waits up to the timeout for a late
arrival:

```csharp
_ = Task.Run(() => fileSystem.File.WriteAllText("help.txt", "help"));
await That(fileSystem).TriggeredNotification().Within(100.Milliseconds());
```

`DidNotTriggerNotification` mirrors the same shapes and short-circuits as soon
as a matching notification is observed:

```csharp
await That(fileSystem).DidNotTriggerNotification().Within(100.Milliseconds());
await That(fileSystem).DidNotTriggerNotification(c => c.Name == "unreleased.txt");
```

Both accept a `Quantifier` (`AtLeast`, `AtMost`, `Exactly`, `Between`,
`Never`, `Once`) so you can assert how often the notification fires, and a
`.Which(c => …)` callback that composes the per-notification expectations
from [`ChangeDescription`](#changedescription):

```csharp
fileSystem.File.WriteAllText("come-together.txt", "come together");
fileSystem.File.WriteAllText("something.txt", "something");

await That(fileSystem).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
    .Exactly(2.Times());

await That(fileSystem)
    .TriggeredNotification()
    .Which(c => c.HasName("come-together.txt").And.HasChangeType(WatcherChangeTypes.Created))
    .Exactly(1.Times());
```

> Replay of historical notifications relies on the `MockFileSystem` notification
> history. Disable it via
> `new MockFileSystem(o => o.WithoutNotificationHistory())` only if you don't
> use these assertions: they throw against a history-disabled file system.

## Watcher events (`IFileSystemWatcher`)

An individual `IFileSystemWatcher` can also be the subject. The watcher must
come from a `MockFileSystem`, and `EnableRaisingEvents` must be `true` for any
event to be observed. Only events fired on this specific watcher count;
events fired on other watchers of the same `MockFileSystem` are ignored.

```csharp
MockFileSystem fileSystem = new();
fileSystem.InitializeIn("/watched");
using IFileSystemWatcher watcher = fileSystem.FileSystemWatcher.New("/watched");
watcher.EnableRaisingEvents = true;
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await That(watcher).Triggered();
await That(watcher).Triggered(c => c.Name == "let-it-be.txt");

await That(watcher).DidNotTrigger().Within(100.Milliseconds());
await That(watcher).DidNotTrigger(c => c.Name == "unreleased.txt");
```

`Triggered` and `DidNotTrigger` share the same shape as the
[notification](#file-system-notifications) assertions: a `Quantifier`
(`AtLeast`, `AtMost`, `Exactly`, `Between`, `Never`, `Once`), a
`.Within(timeout)` (default 30 s), and a `.Which(c => …)` callback that
composes the per-change expectations from
[`ChangeDescription`](#changedescription):

```csharp
await That(watcher)
    .Triggered()
    .Which(c => c.HasName("let-it-be.txt").And.HasChangeType(WatcherChangeTypes.Created))
    .Exactly(1.Times());
```

## `ChangeDescription`

Individual `ChangeDescription` instances can be asserted directly:

```csharp
await That(change).HasChangeType(WatcherChangeTypes.Created);
await That(change).DoesNotHaveChangeType(WatcherChangeTypes.Deleted);

await That(change).HasFileSystemType(FileSystemTypes.File);
await That(change).HasNotifyFilters(NotifyFilters.LastWrite);

await That(change).HasName("let-it-be.txt").And.HasPath("/music/let-it-be.txt");
await That(renamedChange).HasOldName("scrambled-eggs.txt").And.HasOldPath("/music/scrambled-eggs.txt");
```

`HasChangeType`, `HasFileSystemType` and `HasNotifyFilters` use flag
containment (so a `LastWrite | FileName` change satisfies
`HasNotifyFilters(NotifyFilters.LastWrite)`); the empty / `default` value is
rejected with an `ArgumentException` to avoid silent passes.

## Recorded calls (`IFileSystemStatistics`)

`MockFileSystem.Statistics` records every method call and property access on
the mock. `.Recorded()` exposes a fluent mirror over these recordings, so you
can assert what the system under test actually called:

```csharp
MockFileSystem fileSystem = new();
fileSystem.File.WriteAllText("help.txt", "help");

await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
await That(fileSystem.Statistics).Recorded().File.WriteAllText(path: p => p == "help.txt").Once();
```

The mirror has one entry per `IFileSystem` member (`.File`, `.Directory`,
`.FileInfo[path]`, `.DirectoryInfo[path]`, `.DriveInfo`, `.FileStream`,
`.FileSystemWatcher`, `.FileVersionInfo`, `.Path`), with one method per
underlying API and an indexer (`[path]`) for per-instance buckets. Every
result inherits the count vocabulary (`Once`, `Twice`, `Never`, `Exactly`,
`AtLeast`, `AtMost`, `Between`, …).

Like every count in this package, it can be specified only once: chaining two
counts, e.g. `.Once().Twice()`, throws an `InvalidOperationException`; use
`Between(minimum).And(maximum)` for a range.

Property reads and writes are recorded with `.Get()` / `.Set()`:

```csharp
fileSystem.FileInfo.New("help.txt").IsReadOnly = true;

await That(fileSystem.Statistics).Recorded().FileInfo["help.txt"].IsReadOnly.Set().Once();
await That(fileSystem.Statistics).Recorded().DirectoryInfo["beatles"].Exists.Get().AtLeast().Once();
```

Each parameter on a mirror method is an optional `Func<T, bool>` predicate
matched **positionally** against the recorded argument:

- Supplying no predicate (or `null`) skips that position and matches every
  overload, so `.File.Open()` counts _all_ `Open` invocations regardless of arity.
- A predicate whose position exceeds an overload's arity excludes that
  overload, so filtering `recursive` on `Directory.Delete` only matches the
  two-argument overload.
- A predicate whose type differs from the recorded type at that position
  silently excludes that overload, so filtering `searchOption` on
  `Directory.EnumerateDirectories` never matches the `EnumerationOptions`
  overload.

A handful of methods can't be filtered fully through this positional model
because two overloads place different types at the same recording position
(`File.Open` / `FileInfo.Open` with `FileStreamOptions`,
`FileSystemWatcher.WaitForChanged` with `TimeSpan`).

## Timer (`ITimerMock`)

A `MockTimeSystem` exposes timers as `ITimerMock`. You can assert how often
the timer callback was executed without blocking the test thread:

```csharp
MockTimeSystem timeSystem = new();
ITimerMock timer = (ITimerMock)timeSystem.Timer.New(
    _ => { }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));

await That(timer).Executed().AtLeast(3.Times()).Within(5.Seconds());
```

The result of `Executed()` accepts a quantifier (`AtLeast`, `AtMost`,
`Exactly`, `Between`, `Never`, `Once`; at least once by default) and exposes
`.Within(timeout)` for asynchronous execution. The assertion polls
`ITimerMock.ExecutionCount` until the quantifier is satisfied or the timeout
expires (30 seconds by default).

```csharp
await That(timer).Executed().AtLeast(2.Times()).Within(100.Milliseconds());
```
