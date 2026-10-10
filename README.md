# aweXpect.Testably

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)
[![Build](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Testably)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Testably)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Testably%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Testably/main)

Expectations for the file system and time system mocks from
[Testably.Abstractions](https://github.com/Testably/Testably.Abstractions) for
[aweXpect](https://github.com/Testably/aweXpect).

## Overview

| Subject                                         | Expectation                                                | Negated                                      | Summary                                                       |
|-------------------------------------------------|------------------------------------------------------------|----------------------------------------------|---------------------------------------------------------------|
| [`IFileSystem`](#file-system)                   | `HasFile`                                                  | `DoesNotHaveFile`                            | contains the file, with its content and timestamps            |
|                                                 | `HasDirectory`                                             | `DoesNotHaveDirectory`                       | contains the directory, with its files and subdirectories     |
|                                                 | `HasDrive`                                                 | `DoesNotHaveDrive`                           | contains the drive                                            |
| [`IFileInfo`](#file)                            | `Exists`                                                   | `DoesNotExist`                               | the file exists                                               |
|                                                 | `HasName`, `HasExtension`, `HasLength`, `HasContent`       |                                              | has the expected name, extension, length or content           |
|                                                 | `IsReadOnly`                                               | `IsNotReadOnly`                              | the file is read-only                                         |
|                                                 | `HasAttribute`                                             | `DoesNotHaveAttribute`                       | has the attribute flag                                        |
|                                                 | `HasCreationTime`, `HasLastAccessTime`, `HasLastWriteTime` |                                              | has the expected timestamp, optionally within a tolerance     |
| [`IDirectoryInfo`](#directory)                  | `Exists`                                                   | `DoesNotExist`                               | the directory exists                                          |
|                                                 | `IsEmpty`                                                  | `IsNotEmpty`                                 | the directory contains no files or subdirectories             |
|                                                 | `HasFile`, `HasDirectory`                                  | `DoesNotHaveFile`, `DoesNotHaveDirectory`    | contains the file or subdirectory                             |
|                                                 | `HasName`, `HasAttribute`, `Has…Time`                      | `DoesNotHaveAttribute`                       | same as for `IFileInfo`                                       |
| [`IDriveInfo`](#drive)                          | `IsReady`                                                  | `IsNotReady`                                 | the drive is ready                                            |
|                                                 | `HasName`, `HasDriveFormat`, `HasDriveType`, `Has…Size`, … |                                              | has the expected property value                               |
| [`IFileVersionInfo`](#file-version-info)        | `Has…`, `Is…`                                              | `IsNot…`                                     | one expectation per property                                  |
| [`MockFileSystem`](#file-system-notifications)  | `TriggeredNotification`                                    | `DidNotTriggerNotification`                  | raised a matching change notification                         |
| [`IFileSystemWatcher`](#watcher-events)         | `Triggered`                                                | `DidNotTrigger`                              | raised a matching event                                       |
| [`ChangeDescription`](#change-descriptions)     | `HasChangeType`, `HasFileSystemType`, `HasNotifyFilters`   | `DoesNotHave…`                               | has the expected change flags                                 |
|                                                 | `HasName`, `HasPath`, `HasOldName`, `HasOldPath`           |                                              | has the expected (old) name or path                           |
| [`IFileSystemStatistics`](#recorded-calls)      | `Recorded()`                                               |                                              | a method was called or a property was accessed                |
| [`ITimerMock`](#timer)                          | `Executed`                                                 |                                              | the timer callback was executed                               |

The samples use the `TimeSpan` helpers (`1.Second()`, `100.Milliseconds()`) from
[aweXpect.Chronology](https://github.com/Testably/aweXpect.Chronology).

## File system

You can verify that a file, directory or drive exists in an `IFileSystem`, or that it does not:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("beatles/abbey-road");
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await Expect.That(fileSystem).HasDirectory("beatles/abbey-road");
await Expect.That(fileSystem).HasFile("let-it-be.txt");

await Expect.That(fileSystem).DoesNotHaveDirectory("beatles/white-album");
await Expect.That(fileSystem).DoesNotHaveFile("yesterday.txt");
```

For example, `Expect.That(fileSystem).HasFile("yesterday.txt")` fails with:

```text title="Failure message"
Expected that fileSystem
has file 'yesterday.txt',
but it did not exist
```

### File content and timestamps

You can verify the content of the file found by `HasFile(path)`:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent("LET IT BE").IgnoringCase();
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().NotEqualTo("let it go");
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent(Encoding.UTF8.GetBytes("let it be"));
```

The string content supports the same options as
[comparing strings](https://docs.testably.org/aweXpect/values/string#equality). When the content differs, the failure
message shows where and includes the file content:

```text title="Failure message"
Expected that fileSystem
has file 'let-it-be.txt' with content equal to "let it go",
but it was "let it be", which differs at index 7:
          ↓ (actual)
  "let it be"
  "let it go"
          ↑ (expected)

File content:
let it be
```

You can also compare the content with another file on the same file system:

```csharp
fileSystem.File.WriteAllText("let-it-be-remastered.txt", "LET IT BE");
fileSystem.File.WriteAllText("hey-jude.txt", "hey jude");

await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().SameAs("let-it-be-remastered.txt").IgnoringCase();
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent().NotSameAs("hey-jude.txt");
```

You can verify the timestamps of the file, and widen the comparison to a window with `.Within(tolerance)`:

```csharp
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithCreationTime(DateTime.Now).Within(1.Second());
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithLastAccessTime(DateTime.Now).Within(1.Second());
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithLastWriteTime(DateTime.Now).Within(1.Second());
```

### Directory content

You can verify the files and subdirectories of the directory found by `HasDirectory(path)`:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.Directory.CreateDirectory("beatles/abbey-road");
fileSystem.Directory.CreateDirectory("beatles/revolver");
fileSystem.File.WriteAllText("beatles/abbey-road/something.txt", "something");

await Expect.That(fileSystem).HasDirectory("beatles").WithDirectories(d => d.HasCount().EqualTo(2));
await Expect.That(fileSystem).HasDirectory("beatles/abbey-road").WithFiles(f => f
    .All().ComplyWith(x => x.HasContent("SOMETHING").IgnoringCase()));
```

### Continuing with the file, directory or drive

`HasFile`, `HasDirectory` and `HasDrive` each have a `.Which` property that continues with the found `IFileInfo`,
`IDirectoryInfo` or `IDriveInfo`, so that the expectations of the following sections can be used in the same chain:

```csharp
await Expect.That(fileSystem).HasFile("let-it-be.txt").Which.HasLength(9).And.HasContent("let it be");
await Expect.That(fileSystem).HasDirectory("beatles/revolver").Which.IsEmpty();
await Expect.That(fileSystem).HasDrive("D:\\").Which.IsReady().And.HasDriveFormat("NTFS");
```

## File

You can verify the properties of an `IFileInfo`:

```csharp
IFileInfo fileInfo = fileSystem.FileInfo.New("let-it-be.txt");

await Expect.That(fileInfo).Exists();
await Expect.That(fileInfo).DoesNotExist();

await Expect.That(fileInfo).HasName("let-it-be.txt");
await Expect.That(fileInfo).HasExtension(".txt");
await Expect.That(fileInfo).HasLength(9);
await Expect.That(fileInfo).HasContent("let it be");
await Expect.That(fileInfo).HasContent(Encoding.UTF8.GetBytes("let it be"));

await Expect.That(fileInfo).IsReadOnly();
await Expect.That(fileInfo).IsNotReadOnly();

await Expect.That(fileInfo).HasAttribute(FileAttributes.ReadOnly);
await Expect.That(fileInfo).DoesNotHaveAttribute(FileAttributes.Hidden);

await Expect.That(fileInfo).HasCreationTime(DateTime.Now).Within(1.Second());
await Expect.That(fileInfo).HasLastAccessTime(DateTime.Now).Within(1.Second());
await Expect.That(fileInfo).HasLastWriteTime(DateTime.Now).Within(1.Second());
```

For example, `Expect.That(fileInfo).HasLength(10)` fails with:

```text title="Failure message"
Expected that fileInfo
has length 10,
but it was 9
```

`HasAttribute` and `DoesNotHaveAttribute` check whether the flag is contained, so
`FileAttributes.ReadOnly | FileAttributes.Hidden` satisfies `HasAttribute(FileAttributes.ReadOnly)`. The empty
(`default`) value throws an `ArgumentException`, because it would be contained in every value.

On .NET 10 or later, `WhoseParent` continues with the containing directory, so that the
[directory](#directory) expectations can be used:

```csharp
await Expect.That(fileInfo).WhoseParent.HasName("beatles").And.IsNotEmpty();
```

## Directory

You can verify the properties of an `IDirectoryInfo`:

```csharp
IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("beatles");

await Expect.That(dirInfo).Exists();
await Expect.That(dirInfo).DoesNotExist();

await Expect.That(dirInfo).HasName("beatles");

await Expect.That(dirInfo).IsEmpty();
await Expect.That(dirInfo).IsNotEmpty();

await Expect.That(dirInfo).HasFile("abbey-road/something.txt");
await Expect.That(dirInfo).DoesNotHaveFile("abbey-road/yesterday.txt");
await Expect.That(dirInfo).HasDirectory("abbey-road").Which.HasFile("something.txt");
await Expect.That(dirInfo).DoesNotHaveDirectory("white-album");

await Expect.That(dirInfo).HasAttribute(FileAttributes.Directory);
await Expect.That(dirInfo).DoesNotHaveAttribute(FileAttributes.Hidden);

await Expect.That(dirInfo).HasCreationTime(DateTime.Now).Within(1.Second());
await Expect.That(dirInfo).HasLastAccessTime(DateTime.Now).Within(1.Second());
await Expect.That(dirInfo).HasLastWriteTime(DateTime.Now).Within(1.Second());
```

`HasFile` and `HasDirectory` resolve the path relative to the directory and continue like on the
[file system](#file-system), so `.WithContent(…)`, `.WithLastWriteTime(…)`, `.Which` etc. work as well.

On .NET 10 or later, `WhoseParent` continues with the parent directory:

```csharp
await Expect.That(dirInfo).WhoseParent.HasName("…");
```

## Drive

You can verify the properties of an `IDriveInfo`:

```csharp
MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));

IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

await Expect.That(driveInfo).HasAvailableFreeSpace(2048);
await Expect.That(driveInfo).HasTotalSize(2048).And.HasTotalFreeSpace(2048);
await Expect.That(driveInfo).HasDriveFormat("NTFS");
await Expect.That(driveInfo).HasDriveType(DriveType.Fixed);
await Expect.That(driveInfo).HasName(driveInfo.Name).And.HasVolumeLabel(driveInfo.VolumeLabel);
await Expect.That(driveInfo).IsReady();
```

`HasDrive` on the file system matches the drive name case-insensitively against `IFileSystem.DriveInfo.GetDrives()`.
UNC drives are not returned by `GetDrives()` and are therefore not supported by `HasDrive`.

## File version info

You can verify an `IFileVersionInfo` obtained via `MockFileSystem.FileVersionInfo.GetVersionInfo`. Its values come
from `MockFileSystem.WithFileVersionInfo(glob, builder)`:

```csharp
MockFileSystem fileSystem = new();
fileSystem.WithFileVersionInfo("*.dll", v => v
    .SetCompanyName("Apple Corps")
    .SetProductName("Abbey Road")
    .SetFileVersion("1.9.6.9")
    .SetIsDebug(true));
fileSystem.File.WriteAllText("AbbeyRoad.dll", "");

IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("AbbeyRoad.dll");

await Expect.That(info).HasCompanyName("Apple Corps").And.HasProductName("Abbey Road");
await Expect.That(info).HasFileVersion("1.9.6.9").And.HasFileMajorPart(1);
await Expect.That(info).IsDebug().And.IsNotPreRelease();
```

For example, `Expect.That(info).HasCompanyName("EMI")` fails with:

```text title="Failure message"
Expected that info
has company name equal to "EMI",
but it was "Apple Corps", which differs at index 0:
   ↓ (actual)
  "Apple Corps"
  "EMI"
   ↑ (expected)
```

Every `IFileVersionInfo` property has its own expectation: `Has…(string)` for the strings, `Has…(int)` for the version
parts and an `Is…()` / `IsNot…()` pair for the booleans.

| Property             | Expectation                                |
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

A `MockFileSystem` raises notifications when files or directories change. You can verify that the code under test
triggered a notification:

```csharp
MockFileSystem fileSystem = new();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await Expect.That(fileSystem).TriggeredNotification();
await Expect.That(fileSystem).TriggeredNotification(c => c.Name == "let-it-be.txt");
```

With `.Within(timeout)` (default 30 seconds) the expectation waits for asynchronous notifications. If a matching
notification was already triggered, it completes immediately; otherwise it waits up to the timeout:

```csharp
_ = Task.Run(() => fileSystem.File.WriteAllText("help.txt", "help"));
await Expect.That(fileSystem).TriggeredNotification().Within(100.Milliseconds());
```

For example, `Expect.That(fileSystem).TriggeredNotification(c => c.Name == "yesterday.txt").Within(100.Milliseconds())`
fails with:

```text title="Failure message"
Expected that fileSystem
triggered a notification matching c => c.Name == "yesterday.txt" at least once within 0:00.100,
but it was not triggered
```

`DidNotTriggerNotification` has the same overloads and fails as soon as a matching notification is triggered:

```csharp
await Expect.That(fileSystem).DidNotTriggerNotification().Within(100.Milliseconds());
await Expect.That(fileSystem).DidNotTriggerNotification(c => c.Name == "free-as-a-bird.txt");
```

Both accept a quantifier (`AtLeast`, `AtMost`, `Exactly`, `Between`, `Never`, `Once`) to verify how often the
notification was triggered, and a `.Which(c => …)` callback with the
[change description](#change-descriptions) expectations for each notification:

```csharp
fileSystem.File.WriteAllText("come-together.txt", "come together");
fileSystem.File.WriteAllText("something.txt", "something");

await Expect.That(fileSystem).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
    .Exactly(2.Times());

await Expect.That(fileSystem)
    .TriggeredNotification()
    .Which(c => c.HasName("come-together.txt").And.HasChangeType(WatcherChangeTypes.Created))
    .Exactly(1.Times());
```

> These expectations replay the notification history of the `MockFileSystem`. They throw on a file system created
> with `new MockFileSystem(o => o.WithoutNotificationHistory())`.

## Watcher events

You can verify the events of a single `IFileSystemWatcher`. The watcher must come from a `MockFileSystem` and have
`EnableRaisingEvents` set to `true`. Only events of this watcher count; events of other watchers on the same
`MockFileSystem` are ignored.

```csharp
MockFileSystem fileSystem = new();
fileSystem.InitializeIn("/music");
using IFileSystemWatcher watcher = fileSystem.FileSystemWatcher.New("/music");
watcher.EnableRaisingEvents = true;
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await Expect.That(watcher).Triggered();
await Expect.That(watcher).Triggered(c => c.Name == "let-it-be.txt");

await Expect.That(watcher).DidNotTrigger().Within(100.Milliseconds());
await Expect.That(watcher).DidNotTrigger(c => c.Name == "free-as-a-bird.txt");
```

`Triggered` and `DidNotTrigger` support the same quantifiers, `.Within(timeout)` (default 30 seconds) and
`.Which(c => …)` callback as the [notification](#file-system-notifications) expectations:

```csharp
await Expect.That(watcher)
    .Triggered()
    .Which(c => c.HasName("let-it-be.txt").And.HasChangeType(WatcherChangeTypes.Created))
    .Exactly(1.Times());
```

## Change descriptions

You can verify a single `ChangeDescription`:

```csharp
await Expect.That(change).HasChangeType(WatcherChangeTypes.Created);
await Expect.That(change).DoesNotHaveChangeType(WatcherChangeTypes.Deleted);

await Expect.That(change).HasFileSystemType(FileSystemTypes.File);
await Expect.That(change).HasNotifyFilters(NotifyFilters.LastWrite);

await Expect.That(change).HasName("let-it-be.txt").And.HasPath("/music/let-it-be.txt");
await Expect.That(renamedChange).HasOldName("scrambled-eggs.txt").And.HasOldPath("/music/scrambled-eggs.txt");
```

`HasChangeType`, `HasFileSystemType` and `HasNotifyFilters` check whether the flag is contained, so a
`LastWrite | FileName` change satisfies `HasNotifyFilters(NotifyFilters.LastWrite)`. The empty (`default`) value throws
an `ArgumentException`, because it would be contained in every value.

## Recorded calls

`MockFileSystem.Statistics` records every method call and property access on the mock. You can verify what the code
under test called with `.Recorded()`, which mirrors the `IFileSystem` API:

```csharp
MockFileSystem fileSystem = new();
fileSystem.File.WriteAllText("help.txt", "help");

await Expect.That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
await Expect.That(fileSystem.Statistics).Recorded().File.WriteAllText(path: p => p == "help.txt").Once();
```

For example, if `help.txt` was written twice, `Expect.That(fileSystem.Statistics).Recorded().File.WriteAllText().Once()`
fails with:

```text title="Failure message"
Expected that fileSystem.Statistics
recorded a call to File.WriteAllText exactly once,
but it was recorded 2 times
```

The mirror has one entry per `IFileSystem` member (`.File`, `.Directory`, `.FileInfo[path]`, `.DirectoryInfo[path]`,
`.DriveInfo`, `.FileStream`, `.FileSystemWatcher`, `.FileVersionInfo`, `.Path`), with one method per underlying API and
an indexer (`[path]`) for the instances of a path. Every result supports the quantifiers `Once`, `Twice`, `Never`,
`Exactly`, `AtLeast`, `AtMost`, `Between`, …

As for every quantifier in this package, only one can be specified: chaining two, e.g. `.Once().Twice()`, throws an
`InvalidOperationException`; use `Between(minimum).And(maximum)` for a range.

Property reads and writes are recorded with `.Get()` and `.Set()`:

```csharp
fileSystem.FileInfo.New("help.txt").IsReadOnly = true;

await Expect.That(fileSystem.Statistics).Recorded().FileInfo["help.txt"].IsReadOnly.Set().Once();
await Expect.That(fileSystem.Statistics).Recorded().DirectoryInfo["beatles"].Exists.Get().AtLeast().Once();
```

Each parameter of a mirror method is an optional `Func<T, bool>` predicate that is matched **by position** against the
recorded arguments:

- Without a predicate (or with `null`) the position is skipped and every overload matches, so `.File.Open()` counts
  _all_ `Open` calls regardless of their arguments.
- A predicate at a position beyond the number of parameters of an overload excludes that overload, so filtering
  `recursive` on `Directory.Delete` only matches the overload with two parameters.
- A predicate whose type differs from the recorded type at that position excludes that overload, so filtering
  `searchOption` on `Directory.EnumerateDirectories` never matches the `EnumerationOptions` overload.

A few methods cannot be filtered completely by position, because two of their overloads have different types at the
same position (`File.Open` / `FileInfo.Open` with `FileStreamOptions`, `FileSystemWatcher.WaitForChanged` with
`TimeSpan`).

## Timer

A `MockTimeSystem` creates timers as `ITimerMock`. You can verify how often the timer callback was executed without
blocking the test thread:

```csharp
MockTimeSystem timeSystem = new();
ITimerMock timer = (ITimerMock)timeSystem.Timer.New(
    _ => { }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));

await Expect.That(timer).Executed().AtLeast(3.Times()).Within(5.Seconds());
```

`Executed()` accepts a quantifier (`AtLeast`, `AtMost`, `Exactly`, `Between`, `Never`, `Once`; at least once by
default) and `.Within(timeout)` (default 30 seconds) for an asynchronous execution. The expectation polls
`ITimerMock.ExecutionCount` until the quantifier is satisfied or the timeout expires.

For example, for a timer that never fires, `Expect.That(timer).Executed().AtLeast(3.Times()).Within(100.Milliseconds())`
fails with:

```text title="Failure message"
Expected that timer
executed at least 3 times within 0:00.100,
but it was not executed
```
