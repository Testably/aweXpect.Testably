# aweXpect.Testably

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Testably)](https://www.nuget.org/packages/aweXpect.Testably)
[![Build](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Testably/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Testably)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Testably&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Testably)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Testably%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Testably/main)

Expectations for the file system and time system mocks from
[Testably.Abstractions](https://github.com/Testably/Testably.Abstractions) for
[aweXpect](https://github.com/Testably/aweXpect).

## Installation

```shell
dotnet add package aweXpect.Testably
```

The package adds its expectations to aweXpect, so the test project also needs the
[`aweXpect`](https://www.nuget.org/packages/aweXpect) package.

## At a glance

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package
using static aweXpect.Expect;

MockFileSystem fileSystem = new();
fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

// verify that a file exists, together with its content
await That(fileSystem).HasFile("let-it-be.txt").WithContent("LET IT BE").IgnoringCase();

// verify the properties of the found file
await That(fileSystem).HasFile("let-it-be.txt").Which.HasLength(9);

// verify that a file does not exist
await That(fileSystem).DoesNotHaveFile("yesterday.txt");

// verify the notifications the file system raised
await That(fileSystem).TriggeredNotification(c => c.Name == "let-it-be.txt");

// verify the method calls the file system recorded
await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();

// verify how often the callback of a timer from a MockTimeSystem was executed
MockTimeSystem timeSystem = new();
ITimerMock timer = (ITimerMock)timeSystem.Timer.New(_ => { }, null, TimeSpan.Zero, 10.Milliseconds());
await That(timer).Executed().AtLeast(3.Times()).Within(5.Seconds());
```

A failing expectation on the file system names the path it looked for. If the file was never written,
`HasFile("yesterday.txt")` would fail with:

```text title="Failure message"
Expected that fileSystem
has file "yesterday.txt",
but it did not exist
```

## Overview

Expectations for [`IFileSystem`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#files-directories-and-drives):

| Expectation                                                                 | Negated                | Summary                                                              |
|-----------------------------------------------------------------------------|------------------------|----------------------------------------------------------------------|
| [`HasFile`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#files-directories-and-drives)      | `DoesNotHaveFile`      | contains the file, optionally with its content and timestamps        |
| [`HasDirectory`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#files-directories-and-drives) | `DoesNotHaveDirectory` | contains the directory, optionally with its files and subdirectories |
| [`HasDrive`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#files-directories-and-drives)     | `DoesNotHaveDrive`     | contains the drive                                                   |

Expectations for [`IFileInfo`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file):

| Expectation                                              | Negated                   | Summary                                                          |
|----------------------------------------------------------|---------------------------|------------------------------------------------------------------|
| [`Exists`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)            | `DoesNotExist`            | the file exists                                                  |
| [`HasName`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)           |                           | has the expected name                                            |
| [`HasExtension`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)      |                           | has the expected extension                                       |
| [`HasLength`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)         |                           | has the expected length                                          |
| [`HasContent`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)        | `HasContent().NotEqualTo` | has the expected string or binary content                        |
| [`IsReadOnly`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)        | `IsNotReadOnly`           | the file is read-only                                            |
| [`HasAttribute`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)      | `DoesNotHaveAttribute`    | has the attribute flag                                           |
| [`HasCreationTime`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)   |                           | has the expected creation time, optionally within a tolerance    |
| [`HasLastAccessTime`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file) |                           | has the expected last access time, optionally within a tolerance |
| [`HasLastWriteTime`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)  |                           | has the expected last write time, optionally within a tolerance  |
| [`WhoseParent`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file)       |                           | the containing directory meets expectations (.NET 10 or later)   |

Expectations for [`IDirectoryInfo`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory):

| Expectation                                                   | Negated                | Summary                                                          |
|---------------------------------------------------------------|------------------------|------------------------------------------------------------------|
| [`Exists`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)            | `DoesNotExist`         | the directory exists                                             |
| [`HasName`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)           |                        | has the expected name                                            |
| [`IsEmpty`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)           | `IsNotEmpty`           | contains no files or subdirectories                              |
| [`HasFile`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)           | `DoesNotHaveFile`      | contains the file                                                |
| [`HasDirectory`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)      | `DoesNotHaveDirectory` | contains the subdirectory                                        |
| [`HasAttribute`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)      | `DoesNotHaveAttribute` | has the attribute flag                                           |
| [`HasCreationTime`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)   |                        | has the expected creation time, optionally within a tolerance    |
| [`HasLastAccessTime`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory) |                        | has the expected last access time, optionally within a tolerance |
| [`HasLastWriteTime`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)  |                        | has the expected last write time, optionally within a tolerance  |
| [`WhoseParent`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#directory)       |                        | the parent directory meets expectations (.NET 10 or later)       |

Expectations for [`IDriveInfo`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive):

| Expectation                                                   | Negated      | Summary                               |
|---------------------------------------------------------------|--------------|---------------------------------------|
| [`IsReady`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive)               | `IsNotReady` | the drive is ready                    |
| [`HasName`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive)               |              | has the expected name                 |
| [`HasVolumeLabel`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive)        |              | has the expected volume label         |
| [`HasDriveFormat`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive)        |              | has the expected drive format         |
| [`HasDriveType`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive)          |              | has the expected drive type           |
| [`HasTotalSize`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive)          |              | has the expected total size           |
| [`HasTotalFreeSpace`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive)     |              | has the expected total free space     |
| [`HasAvailableFreeSpace`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#drive) |              | has the expected available free space |

Expectations for [`IFileVersionInfo`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file-version-info):

| Expectation                                                                                          | Negated         | Summary                      |
|------------------------------------------------------------------------------------------------------|-----------------|------------------------------|
| [`HasCompanyName`, `HasFileMajorPart`, `IsDebug`, …](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system#file-version-info) | `IsNotDebug`, … | one expectation per property |

Expectations for [`MockFileSystem`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#file-system-notifications):

| Expectation                                                                         | Negated                     | Summary                                        |
|-------------------------------------------------------------------------------------|-----------------------------|------------------------------------------------|
| [`TriggeredNotification`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#file-system-notifications) | `DidNotTriggerNotification` | the file system raised a matching notification |

Expectations for [`IFileSystemWatcher`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#watcher-events):

| Expectation                                                  | Negated         | Summary                             |
|--------------------------------------------------------------|-----------------|-------------------------------------|
| [`Triggered`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#watcher-events) | `DidNotTrigger` | the watcher raised a matching event |

Expectations for [`ChangeDescription`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#change-descriptions):

| Expectation                                                               | Negated                     | Summary                                                  |
|---------------------------------------------------------------------------|-----------------------------|----------------------------------------------------------|
| [`HasChangeType`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#change-descriptions)     | `DoesNotHaveChangeType`     | has the change type flag                                 |
| [`HasFileSystemType`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#change-descriptions) | `DoesNotHaveFileSystemType` | has the file system type flag                            |
| [`HasNotifyFilters`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#change-descriptions)  | `DoesNotHaveNotifyFilters`  | has the notify filters flag                              |
| [`HasName`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#change-descriptions)           |                             | has the expected name                                    |
| [`HasPath`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#change-descriptions)           |                             | has the expected path                                    |
| [`HasOldName`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#change-descriptions)        |                             | has the expected old name of a renamed file or directory |
| [`HasOldPath`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#change-descriptions)        |                             | has the expected old path of a renamed file or directory |

Expectations for [`IFileSystemStatistics`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#recorded-calls):

| Expectation                                                 | Negated    | Summary                                        |
|-------------------------------------------------------------|------------|------------------------------------------------|
| [`Recorded`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications#recorded-calls) | `.Never()` | a method was called or a property was accessed |

Expectations for [`ITimerMock`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/time-system#timer):

| Expectation                                      | Negated    | Summary                         |
|--------------------------------------------------|------------|---------------------------------|
| [`Executed`](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/time-system#timer) | `.Never()` | the timer callback was executed |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/):

- [File system](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/file-system): files, directories and drives of an `IFileSystem`, and the expectations
  on `IFileInfo`, `IDirectoryInfo`, `IDriveInfo` and `IFileVersionInfo`
- [Notifications and recorded calls](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/notifications): file-system notifications, watcher events,
  change descriptions and the calls recorded in `IFileSystemStatistics`
- [Time system](https://docs.testably.org/aweXpect/extensions/aweXpect.Testably/time-system): the execution of `ITimerMock` timers
