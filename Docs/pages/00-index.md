import PackageInstall from '@site/src/components/PackageInstall';

# aweXpect.Testably

[aweXpect.Testably](https://github.com/Testably/aweXpect.Testably) contains expectations for the file system and time
system mocks from [Testably.Abstractions](/Abstractions). It is an extension for [aweXpect](/aweXpect).

## Installation

<PackageInstall package="aweXpect.Testably" />

The package adds its expectations to aweXpect, so the test project also needs the
[`aweXpect`](/aweXpect/getting-started) package.

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

Expectations for [`IFileSystem`](./01-file-system.md#files-directories-and-drives):

| Expectation                                                     | Negated                | Summary                                                              |
|-----------------------------------------------------------------|------------------------|----------------------------------------------------------------------|
| [`HasFile`](./01-file-system.md#files-directories-and-drives)      | `DoesNotHaveFile`      | contains the file, optionally with its content and timestamps        |
| [`HasDirectory`](./01-file-system.md#files-directories-and-drives) | `DoesNotHaveDirectory` | contains the directory, optionally with its files and subdirectories |
| [`HasDrive`](./01-file-system.md#files-directories-and-drives)     | `DoesNotHaveDrive`     | contains the drive                                                   |

Expectations for [`IFileInfo`](./01-file-system.md#file):

| Expectation                                         | Negated                   | Summary                                                          |
|-----------------------------------------------------|---------------------------|------------------------------------------------------------------|
| [`Exists`](./01-file-system.md#file)                | `DoesNotExist`            | the file exists                                                  |
| [`HasName`](./01-file-system.md#file)               |                           | has the expected name                                            |
| [`HasExtension`](./01-file-system.md#file)          |                           | has the expected extension                                       |
| [`HasLength`](./01-file-system.md#file)             |                           | has the expected length                                          |
| [`HasContent`](./01-file-system.md#file)            | `HasContent().NotEqualTo` | has the expected string or binary content                        |
| [`IsReadOnly`](./01-file-system.md#file)            | `IsNotReadOnly`           | the file is read-only                                            |
| [`HasAttribute`](./01-file-system.md#file)          | `DoesNotHaveAttribute`    | has the attribute flag                                           |
| [`HasCreationTime`](./01-file-system.md#file)       |                           | has the expected creation time, optionally within a tolerance    |
| [`HasLastAccessTime`](./01-file-system.md#file)     |                           | has the expected last access time, optionally within a tolerance |
| [`HasLastWriteTime`](./01-file-system.md#file)      |                           | has the expected last write time, optionally within a tolerance  |
| [`WhoseParent`](./01-file-system.md#file)           |                           | the containing directory meets expectations (.NET 10 or later)   |

Expectations for [`IDirectoryInfo`](./01-file-system.md#directory):

| Expectation                                         | Negated                | Summary                                                          |
|-----------------------------------------------------|------------------------|------------------------------------------------------------------|
| [`Exists`](./01-file-system.md#directory)            | `DoesNotExist`         | the directory exists                                             |
| [`HasName`](./01-file-system.md#directory)           |                        | has the expected name                                            |
| [`IsEmpty`](./01-file-system.md#directory)           | `IsNotEmpty`           | contains no files or subdirectories                              |
| [`HasFile`](./01-file-system.md#directory)           | `DoesNotHaveFile`      | contains the file                                                |
| [`HasDirectory`](./01-file-system.md#directory)      | `DoesNotHaveDirectory` | contains the subdirectory                                        |
| [`HasAttribute`](./01-file-system.md#directory)      | `DoesNotHaveAttribute` | has the attribute flag                                           |
| [`HasCreationTime`](./01-file-system.md#directory)   |                        | has the expected creation time, optionally within a tolerance    |
| [`HasLastAccessTime`](./01-file-system.md#directory) |                        | has the expected last access time, optionally within a tolerance |
| [`HasLastWriteTime`](./01-file-system.md#directory)  |                        | has the expected last write time, optionally within a tolerance  |
| [`WhoseParent`](./01-file-system.md#directory)       |                        | the parent directory meets expectations (.NET 10 or later)       |

Expectations for [`IDriveInfo`](./01-file-system.md#drive):

| Expectation                                         | Negated      | Summary                               |
|-----------------------------------------------------|--------------|---------------------------------------|
| [`IsReady`](./01-file-system.md#drive)               | `IsNotReady` | the drive is ready                    |
| [`HasName`](./01-file-system.md#drive)               |              | has the expected name                 |
| [`HasVolumeLabel`](./01-file-system.md#drive)        |              | has the expected volume label         |
| [`HasDriveFormat`](./01-file-system.md#drive)        |              | has the expected drive format         |
| [`HasDriveType`](./01-file-system.md#drive)          |              | has the expected drive type           |
| [`HasTotalSize`](./01-file-system.md#drive)          |              | has the expected total size           |
| [`HasTotalFreeSpace`](./01-file-system.md#drive)     |              | has the expected total free space     |
| [`HasAvailableFreeSpace`](./01-file-system.md#drive) |              | has the expected available free space |

Expectations for [`IFileVersionInfo`](./01-file-system.md#file-version-info):

| Expectation                                                                       | Negated         | Summary                      |
|-----------------------------------------------------------------------------------|-----------------|------------------------------|
| [`HasCompanyName`, `HasFileMajorPart`, `IsDebug`, …](./01-file-system.md#file-version-info) | `IsNotDebug`, … | one expectation per property |

Expectations for [`MockFileSystem`](./02-notifications.md#file-system-notifications):

| Expectation                                                           | Negated                     | Summary                                        |
|-----------------------------------------------------------------------|-----------------------------|------------------------------------------------|
| [`TriggeredNotification`](./02-notifications.md#file-system-notifications) | `DidNotTriggerNotification` | the file system raised a matching notification |

Expectations for [`IFileSystemWatcher`](./02-notifications.md#watcher-events):

| Expectation                                       | Negated         | Summary                             |
|---------------------------------------------------|-----------------|-------------------------------------|
| [`Triggered`](./02-notifications.md#watcher-events) | `DidNotTrigger` | the watcher raised a matching event |

Expectations for [`ChangeDescription`](./02-notifications.md#change-descriptions):

| Expectation                                                | Negated                     | Summary                                                  |
|------------------------------------------------------------|-----------------------------|----------------------------------------------------------|
| [`HasChangeType`](./02-notifications.md#change-descriptions)     | `DoesNotHaveChangeType`     | has the change type flag                                 |
| [`HasFileSystemType`](./02-notifications.md#change-descriptions) | `DoesNotHaveFileSystemType` | has the file system type flag                            |
| [`HasNotifyFilters`](./02-notifications.md#change-descriptions)  | `DoesNotHaveNotifyFilters`  | has the notify filters flag                              |
| [`HasName`](./02-notifications.md#change-descriptions)           |                             | has the expected name                                    |
| [`HasPath`](./02-notifications.md#change-descriptions)           |                             | has the expected path                                    |
| [`HasOldName`](./02-notifications.md#change-descriptions)        |                             | has the expected old name of a renamed file or directory |
| [`HasOldPath`](./02-notifications.md#change-descriptions)        |                             | has the expected old path of a renamed file or directory |

Expectations for [`IFileSystemStatistics`](./02-notifications.md#recorded-calls):

| Expectation                                     | Negated    | Summary                                        |
|-------------------------------------------------|------------|------------------------------------------------|
| [`Recorded`](./02-notifications.md#recorded-calls) | `.Never()` | a method was called or a property was accessed |

Expectations for [`ITimerMock`](./03-time-system.md#timer):

| Expectation                            | Negated    | Summary                         |
|----------------------------------------|------------|---------------------------------|
| [`Executed`](./03-time-system.md#timer) | `.Never()` | the timer callback was executed |
