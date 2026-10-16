# aweXpect.Testably

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)
[![Build](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Testably)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Testably)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Testably%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Testably/main)

Expectations for the file system and time system mocks from
[Testably.Abstractions](https://github.com/Testably/Testably.Abstractions) for
[aweXpect](https://github.com/Testably/aweXpect).

## At a glance

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

MockFileSystem fileSystem = new();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

// verify that a file exists, together with its content
await Expect.That(fileSystem).HasFile("let-it-be.txt").WithContent("LET IT BE").IgnoringCase();

// verify the properties of the found file
await Expect.That(fileSystem).HasFile("let-it-be.txt").Which.HasLength(9);

// verify that a file does not exist
await Expect.That(fileSystem).DoesNotHaveFile("yesterday.txt");

// verify the notifications the file system raised
await Expect.That(fileSystem).TriggeredNotification(c => c.Name == "let-it-be.txt");

// verify the method calls the file system recorded
await Expect.That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();

// verify how often the callback of a timer from a MockTimeSystem was executed
MockTimeSystem timeSystem = new();
ITimerMock timer = (ITimerMock)timeSystem.Timer.New(_ => { }, null, TimeSpan.Zero, 10.Milliseconds());
await Expect.That(timer).Executed().AtLeast(3.Times()).Within(5.Seconds());
```

A failing expectation on the file system names the path it looked for. If the file was never written,
`HasFile("yesterday.txt")` would fail with:

```text title="Failure message"
Expected that fileSystem
has file "yesterday.txt",
but it did not exist
```

## Overview

Expectations for [`IFileSystem`](Docs/pages/01-file-system.md#files-directories-and-drives):

| Expectation                                                                 | Negated                | Summary                                                              |
|-----------------------------------------------------------------------------|------------------------|----------------------------------------------------------------------|
| [`HasFile`](Docs/pages/01-file-system.md#files-directories-and-drives)      | `DoesNotHaveFile`      | contains the file, optionally with its content and timestamps        |
| [`HasDirectory`](Docs/pages/01-file-system.md#files-directories-and-drives) | `DoesNotHaveDirectory` | contains the directory, optionally with its files and subdirectories |
| [`HasDrive`](Docs/pages/01-file-system.md#files-directories-and-drives)     | `DoesNotHaveDrive`     | contains the drive                                                   |

Expectations for [`IFileInfo`](Docs/pages/01-file-system.md#file):

| Expectation                                              | Negated                   | Summary                                                          |
|----------------------------------------------------------|---------------------------|------------------------------------------------------------------|
| [`Exists`](Docs/pages/01-file-system.md#file)            | `DoesNotExist`            | the file exists                                                  |
| [`HasName`](Docs/pages/01-file-system.md#file)           |                           | has the expected name                                            |
| [`HasExtension`](Docs/pages/01-file-system.md#file)      |                           | has the expected extension                                       |
| [`HasLength`](Docs/pages/01-file-system.md#file)         |                           | has the expected length                                          |
| [`HasContent`](Docs/pages/01-file-system.md#file)        | `HasContent().NotEqualTo` | has the expected string or binary content                        |
| [`IsReadOnly`](Docs/pages/01-file-system.md#file)        | `IsNotReadOnly`           | the file is read-only                                            |
| [`HasAttribute`](Docs/pages/01-file-system.md#file)      | `DoesNotHaveAttribute`    | has the attribute flag                                           |
| [`HasCreationTime`](Docs/pages/01-file-system.md#file)   |                           | has the expected creation time, optionally within a tolerance    |
| [`HasLastAccessTime`](Docs/pages/01-file-system.md#file) |                           | has the expected last access time, optionally within a tolerance |
| [`HasLastWriteTime`](Docs/pages/01-file-system.md#file)  |                           | has the expected last write time, optionally within a tolerance  |
| [`WhoseParent`](Docs/pages/01-file-system.md#file)       |                           | the containing directory meets expectations (.NET 10 or later)   |

Expectations for [`IDirectoryInfo`](Docs/pages/01-file-system.md#directory):

| Expectation                                                   | Negated                | Summary                                                          |
|---------------------------------------------------------------|------------------------|------------------------------------------------------------------|
| [`Exists`](Docs/pages/01-file-system.md#directory)            | `DoesNotExist`         | the directory exists                                             |
| [`HasName`](Docs/pages/01-file-system.md#directory)           |                        | has the expected name                                            |
| [`IsEmpty`](Docs/pages/01-file-system.md#directory)           | `IsNotEmpty`           | contains no files or subdirectories                              |
| [`HasFile`](Docs/pages/01-file-system.md#directory)           | `DoesNotHaveFile`      | contains the file                                                |
| [`HasDirectory`](Docs/pages/01-file-system.md#directory)      | `DoesNotHaveDirectory` | contains the subdirectory                                        |
| [`HasAttribute`](Docs/pages/01-file-system.md#directory)      | `DoesNotHaveAttribute` | has the attribute flag                                           |
| [`HasCreationTime`](Docs/pages/01-file-system.md#directory)   |                        | has the expected creation time, optionally within a tolerance    |
| [`HasLastAccessTime`](Docs/pages/01-file-system.md#directory) |                        | has the expected last access time, optionally within a tolerance |
| [`HasLastWriteTime`](Docs/pages/01-file-system.md#directory)  |                        | has the expected last write time, optionally within a tolerance  |
| [`WhoseParent`](Docs/pages/01-file-system.md#directory)       |                        | the parent directory meets expectations (.NET 10 or later)       |

Expectations for [`IDriveInfo`](Docs/pages/01-file-system.md#drive):

| Expectation                                                   | Negated      | Summary                               |
|---------------------------------------------------------------|--------------|---------------------------------------|
| [`IsReady`](Docs/pages/01-file-system.md#drive)               | `IsNotReady` | the drive is ready                    |
| [`HasName`](Docs/pages/01-file-system.md#drive)               |              | has the expected name                 |
| [`HasVolumeLabel`](Docs/pages/01-file-system.md#drive)        |              | has the expected volume label         |
| [`HasDriveFormat`](Docs/pages/01-file-system.md#drive)        |              | has the expected drive format         |
| [`HasDriveType`](Docs/pages/01-file-system.md#drive)          |              | has the expected drive type           |
| [`HasTotalSize`](Docs/pages/01-file-system.md#drive)          |              | has the expected total size           |
| [`HasTotalFreeSpace`](Docs/pages/01-file-system.md#drive)     |              | has the expected total free space     |
| [`HasAvailableFreeSpace`](Docs/pages/01-file-system.md#drive) |              | has the expected available free space |

Expectations for [`IFileVersionInfo`](Docs/pages/01-file-system.md#file-version-info):

| Expectation                                                                                          | Negated         | Summary                      |
|------------------------------------------------------------------------------------------------------|-----------------|------------------------------|
| [`HasCompanyName`, `HasFileMajorPart`, `IsDebug`, …](Docs/pages/01-file-system.md#file-version-info) | `IsNotDebug`, … | one expectation per property |

Expectations for [`MockFileSystem`](Docs/pages/02-notifications.md#file-system-notifications):

| Expectation                                                                         | Negated                     | Summary                                        |
|-------------------------------------------------------------------------------------|-----------------------------|------------------------------------------------|
| [`TriggeredNotification`](Docs/pages/02-notifications.md#file-system-notifications) | `DidNotTriggerNotification` | the file system raised a matching notification |

Expectations for [`IFileSystemWatcher`](Docs/pages/02-notifications.md#watcher-events):

| Expectation                                                  | Negated         | Summary                             |
|--------------------------------------------------------------|-----------------|-------------------------------------|
| [`Triggered`](Docs/pages/02-notifications.md#watcher-events) | `DidNotTrigger` | the watcher raised a matching event |

Expectations for [`ChangeDescription`](Docs/pages/02-notifications.md#change-descriptions):

| Expectation                                                               | Negated                     | Summary                                                  |
|---------------------------------------------------------------------------|-----------------------------|----------------------------------------------------------|
| [`HasChangeType`](Docs/pages/02-notifications.md#change-descriptions)     | `DoesNotHaveChangeType`     | has the change type flag                                 |
| [`HasFileSystemType`](Docs/pages/02-notifications.md#change-descriptions) | `DoesNotHaveFileSystemType` | has the file system type flag                            |
| [`HasNotifyFilters`](Docs/pages/02-notifications.md#change-descriptions)  | `DoesNotHaveNotifyFilters`  | has the notify filters flag                              |
| [`HasName`](Docs/pages/02-notifications.md#change-descriptions)           |                             | has the expected name                                    |
| [`HasPath`](Docs/pages/02-notifications.md#change-descriptions)           |                             | has the expected path                                    |
| [`HasOldName`](Docs/pages/02-notifications.md#change-descriptions)        |                             | has the expected old name of a renamed file or directory |
| [`HasOldPath`](Docs/pages/02-notifications.md#change-descriptions)        |                             | has the expected old path of a renamed file or directory |

Expectations for [`IFileSystemStatistics`](Docs/pages/02-notifications.md#recorded-calls):

| Expectation                                                 | Negated    | Summary                                        |
|-------------------------------------------------------------|------------|------------------------------------------------|
| [`Recorded`](Docs/pages/02-notifications.md#recorded-calls) | `.Never()` | a method was called or a property was accessed |

Expectations for [`ITimerMock`](Docs/pages/03-time-system.md#timer):

| Expectation                                      | Negated    | Summary                         |
|--------------------------------------------------|------------|---------------------------------|
| [`Executed`](Docs/pages/03-time-system.md#timer) | `.Never()` | the timer callback was executed |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/):

- [File system](Docs/pages/01-file-system.md): files, directories and drives of an `IFileSystem`, and the expectations
  on `IFileInfo`, `IDirectoryInfo`, `IDriveInfo` and `IFileVersionInfo`
- [Notifications and recorded calls](Docs/pages/02-notifications.md): file-system notifications, watcher events,
  change descriptions and the calls recorded in `IFileSystemStatistics`
- [Time system](Docs/pages/03-time-system.md): the execution of `ITimerMock` timers
