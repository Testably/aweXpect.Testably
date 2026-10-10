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

| Subject                                                                      | Expectation                                                | Negated                                   | Summary                                                   |
|------------------------------------------------------------------------------|------------------------------------------------------------|-------------------------------------------|-----------------------------------------------------------|
| [`IFileSystem`](Docs/pages/01-file-system.md#files-directories-and-drives)   | `HasFile`                                                  | `DoesNotHaveFile`                         | contains the file, with its content and timestamps        |
|                                                                              | `HasDirectory`                                             | `DoesNotHaveDirectory`                    | contains the directory, with its files and subdirectories |
|                                                                              | `HasDrive`                                                 | `DoesNotHaveDrive`                        | contains the drive                                        |
| [`IFileInfo`](Docs/pages/01-file-system.md#file)                             | `Exists`                                                   | `DoesNotExist`                            | the file exists                                           |
|                                                                              | `HasName`, `HasExtension`, `HasLength`, `HasContent`       |                                           | has the expected name, extension, length or content       |
|                                                                              | `IsReadOnly`                                               | `IsNotReadOnly`                           | the file is read-only                                     |
|                                                                              | `HasAttribute`                                             | `DoesNotHaveAttribute`                    | has the attribute flag                                    |
|                                                                              | `HasCreationTime`, `HasLastAccessTime`, `HasLastWriteTime` |                                           | has the expected timestamp, optionally within a tolerance |
| [`IDirectoryInfo`](Docs/pages/01-file-system.md#directory)                   | `Exists`                                                   | `DoesNotExist`                            | the directory exists                                      |
|                                                                              | `IsEmpty`                                                  | `IsNotEmpty`                              | the directory contains no files or subdirectories         |
|                                                                              | `HasFile`, `HasDirectory`                                  | `DoesNotHaveFile`, `DoesNotHaveDirectory` | contains the file or subdirectory                         |
|                                                                              | `HasName`, `HasAttribute`, `Has…Time`                      | `DoesNotHaveAttribute`                    | same as for `IFileInfo`                                   |
| [`IDriveInfo`](Docs/pages/01-file-system.md#drive)                           | `IsReady`                                                  | `IsNotReady`                              | the drive is ready                                        |
|                                                                              | `HasName`, `HasDriveFormat`, `HasDriveType`, `Has…Size`, … |                                           | has the expected property value                           |
| [`IFileVersionInfo`](Docs/pages/01-file-system.md#file-version-info)         | `Has…`, `Is…`                                              | `IsNot…`                                  | one expectation per property                              |
| [`MockFileSystem`](Docs/pages/02-notifications.md#file-system-notifications) | `TriggeredNotification`                                    | `DidNotTriggerNotification`               | raised a matching change notification                     |
| [`IFileSystemWatcher`](Docs/pages/02-notifications.md#watcher-events)        | `Triggered`                                                | `DidNotTrigger`                           | raised a matching event                                   |
| [`ChangeDescription`](Docs/pages/02-notifications.md#change-descriptions)    | `HasChangeType`, `HasFileSystemType`, `HasNotifyFilters`   | `DoesNotHave…`                            | has the expected change flags                             |
|                                                                              | `HasName`, `HasPath`, `HasOldName`, `HasOldPath`           |                                           | has the expected (old) name or path                       |
| [`IFileSystemStatistics`](Docs/pages/02-notifications.md#recorded-calls)     | `Recorded()`                                               |                                           | a method was called or a property was accessed            |
| [`ITimerMock`](Docs/pages/03-time-system.md#timer)                           | `Executed`                                                 |                                           | the timer callback was executed                           |

The samples use the `TimeSpan` helpers (e.g. `5.Seconds()`) from
[aweXpect.Chronology](https://github.com/Testably/aweXpect.Chronology).

## File system

You can verify that a file exists in an `IFileSystem`, together with its content, and continue with the found
`IFileInfo`:

```csharp
IFileSystem fileSystem = new MockFileSystem();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent("LET IT BE").IgnoringCase();
await Expect.That(fileSystem).HasFile("let-it-be.txt").Which.HasLength(9);
await Expect.That(fileSystem).DoesNotHaveFile("yesterday.txt");
```

For example, `Expect.That(fileSystem).HasFile("yesterday.txt")` fails with:

```text title="Failure message"
Expected that fileSystem
has file 'yesterday.txt',
but it did not exist
```

Directories, drives and file version infos are covered in [File system](Docs/pages/01-file-system.md).

## Notifications and recorded calls

You can verify the notifications a `MockFileSystem` raised and the calls it recorded:

```csharp
MockFileSystem fileSystem = new();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

await Expect.That(fileSystem).TriggeredNotification(c => c.Name == "let-it-be.txt");
await Expect.That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
```

Waiting for asynchronous notifications, watcher events and filtering recorded calls are covered in
[Notifications and recorded calls](Docs/pages/02-notifications.md).

## Time system

You can verify how often the callback of a timer from a `MockTimeSystem` was executed:

```csharp
MockTimeSystem timeSystem = new();
ITimerMock timer = (ITimerMock)timeSystem.Timer.New(
    _ => { }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));

await Expect.That(timer).Executed().AtLeast(3.Times()).Within(5.Seconds());
```

See [Time system](Docs/pages/03-time-system.md) for the details.

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/):

- [File system](Docs/pages/01-file-system.md): files, directories and drives of an `IFileSystem`, and the expectations
  on `IFileInfo`, `IDirectoryInfo`, `IDriveInfo` and `IFileVersionInfo`
- [Notifications and recorded calls](Docs/pages/02-notifications.md): file-system notifications, watcher events,
  change descriptions and the calls recorded in `IFileSystemStatistics`
- [Time system](Docs/pages/03-time-system.md): the execution of `ITimerMock` timers
