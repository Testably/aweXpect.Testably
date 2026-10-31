---
description: Verify the notifications a MockFileSystem raised, the events of a file system watcher, change descriptions and the calls recorded in its statistics.
---

# Notifications and recorded calls

Describes the possible expectations for the changes the code under test made to a `MockFileSystem`: the notifications
it raised, the events of a single watcher, and the method calls and property accesses it recorded.

| Expectation                                           | Negated                     | Summary                                                               |
|-------------------------------------------------------|-----------------------------|-----------------------------------------------------------------------|
| [`TriggeredNotification`](#file-system-notifications) | `DidNotTriggerNotification` | the `MockFileSystem` raised a matching notification                   |
| [`Triggered`](#watcher-events)                        | `DidNotTrigger`             | the `IFileSystemWatcher` raised a matching event                      |
| [`HasChangeType`](#change-descriptions)               | `DoesNotHaveChangeType`     | the change has the change type flag                                   |
| [`HasFileSystemType`](#change-descriptions)           | `DoesNotHaveFileSystemType` | the change has the file system type flag                              |
| [`HasNotifyFilters`](#change-descriptions)            | `DoesNotHaveNotifyFilters`  | the change has the notify filters flag                                |
| [`HasName`](#change-descriptions)                     |                             | the change has the expected name                                      |
| [`HasPath`](#change-descriptions)                     |                             | the change has the expected path                                      |
| [`HasOldName`](#change-descriptions)                  |                             | the renamed change has the expected old name                          |
| [`HasOldPath`](#change-descriptions)                  |                             | the renamed change has the expected old path                          |
| [`Recorded`](#recorded-calls)                         | `.Never()`                  | the `IFileSystemStatistics` recorded a method call or property access |

The samples on this page use the following file system and watcher:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package
using static aweXpect.Expect;

MockFileSystem fileSystem = new();
fileSystem.InitializeIn("/music");
using IFileSystemWatcher watcher = fileSystem.FileSystemWatcher.New("/music");
watcher.EnableRaisingEvents = true;

fileSystem.File.WriteAllText("let-it-be.txt", "let it be");
fileSystem.File.WriteAllText("scrambled-eggs.txt", "scrambled eggs");
fileSystem.File.Move("scrambled-eggs.txt", "yesterday.txt");
```

## File-system notifications

A `MockFileSystem` raises notifications when files or directories change. You can verify that the code under test
triggered a notification, or that it did not:

```csharp
await That(fileSystem).TriggeredNotification();
await That(fileSystem).TriggeredNotification(c => c.Name == "let-it-be.txt");

await That(fileSystem).DidNotTriggerNotification(c => c.Name == "free-as-a-bird.txt");
```

Both accept a quantifier (`AtLeast`, `AtMost`, `Exactly`, `Between`, `Never`, `Once`; at least once by default) to
verify how often the notification was triggered, and a `.Matching(c => …)` filter with the
[change description](#change-descriptions) expectations for each notification:

```csharp
await That(fileSystem).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Renamed).Once();

await That(fileSystem)
    .TriggeredNotification()
    .Matching(c => c.HasName("let-it-be.txt").And.HasChangeType(WatcherChangeTypes.Created))
    .Exactly(1.Times());
```

Without `.Within(timeout)`, the expectations check only the notifications triggered so far and do not wait. With `.Within(timeout)`, they also wait for asynchronous notifications, like [waiting for events](/aweXpect/how-it-works/time-and-cancellation#events): `TriggeredNotification` completes as soon as the quantifier is decided, so only an upper bound like `.Never()` or `.AtMost(…)` waits for the full timeout, and `DidNotTriggerNotification` waits for the full timeout and fails as soon as a matching notification is triggered. `.Within(Timeout.InfiniteTimeSpan)` waits until the quantifier is decided.

```csharp
_ = Task.Run(() => fileSystem.File.Delete("let-it-be.txt"));

await That(fileSystem).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Deleted)
    .Within(100.Milliseconds());
await That(fileSystem).DidNotTriggerNotification(c => c.Name == "free-as-a-bird.txt")
    .Within(100.Milliseconds());
```

For example, `That(fileSystem).TriggeredNotification(c => c.Name == "free-as-a-bird.txt").Within(100.Milliseconds())`
fails with:

```text title="Failure message"
Expected that fileSystem
has triggered a notification matching c => c.Name == "free-as-a-bird.txt" at least once within 0:00.100,
but it was not triggered
```

A timeout of the evaluation (e.g. `.WithTimeout(…)`) that is not shorter than `.Within(timeout)` lets the notifications received until then decide; a cancellation before that leaves the expectation [inconclusive](/aweXpect/how-it-works/time-and-cancellation#outcome).

:::warning[Notification history]
These expectations replay the notification history of the `MockFileSystem`. They throw on a file system created with
`new MockFileSystem(o => o.WithoutNotificationHistory())`.
:::

## Watcher events

You can verify the events of a single `IFileSystemWatcher`, similar to the [events](/aweXpect/behavior/events) of any other object. The watcher must come from a `MockFileSystem` and have `EnableRaisingEvents` set to `true`. Only events of this watcher count; events of other watchers on the same `MockFileSystem` are ignored.

```csharp
await That(watcher).Triggered().Within(1.Seconds());
await That(watcher).Triggered(c => c.Name == "let-it-be.txt").Within(1.Seconds());

await That(watcher).DidNotTrigger(c => c.Name == "free-as-a-bird.txt").Within(100.Milliseconds());
```

The mock raises the events of a watcher asynchronously, like a real `FileSystemWatcher`, so an event of a change that
just completed may not be raised yet. Without `.Within(timeout)`, only the events raised so far count; use it to wait
for them. `Triggered()` completes as soon as a matching event is raised, so a generous timeout only costs time when the
expectation fails.

`Triggered` and `DidNotTrigger` support the same quantifiers, `.Within(timeout)` and `.Matching(c => …)` filter as the
[notification](#file-system-notifications) expectations:

```csharp
await That(watcher)
    .Triggered()
    .Matching(c => c.HasName("let-it-be.txt").And.HasChangeType(WatcherChangeTypes.Created))
    .Exactly(1.Times())
    .Within(1.Seconds());
```

## Change descriptions

You can verify a single `ChangeDescription`, e.g. a notification in `.Matching(…)` or the event of a watcher:

```csharp
await That(fileSystem).TriggeredNotification().Matching(change => change
    .HasChangeType(WatcherChangeTypes.Created).And
    .DoesNotHaveChangeType(WatcherChangeTypes.Deleted).And
    .HasFileSystemType(FileSystemTypes.File).And
    .HasNotifyFilters(NotifyFilters.FileName).And
    .HasName("let-it-be.txt").And
    .HasPath(fileSystem.Path.GetFullPath("let-it-be.txt")));

await That(fileSystem).TriggeredNotification().Matching(renamedChange => renamedChange
    .HasChangeType(WatcherChangeTypes.Renamed).And
    .HasOldName("scrambled-eggs.txt").And
    .HasOldPath(fileSystem.Path.GetFullPath("scrambled-eggs.txt")));
```

`HasChangeType`, `HasFileSystemType` and `HasNotifyFilters` check whether the flag is contained, so a
`LastWrite | FileName` change satisfies `HasNotifyFilters(NotifyFilters.LastWrite)`. The empty (`default`) value throws
an `ArgumentException`, because it would be contained in every value.

## Recorded calls

`MockFileSystem.Statistics` records every method call and property access on the mock. You can verify what the code
under test called with `.Recorded()`, which mirrors the `IFileSystem` API:

```csharp
await That(fileSystem.Statistics).Recorded().File.WriteAllText().Twice();
await That(fileSystem.Statistics).Recorded().File.WriteAllText(path: p => p == "let-it-be.txt").Once();
await That(fileSystem.Statistics).Recorded().File.Copy().Never();
```

For example, `That(fileSystem.Statistics).Recorded().File.WriteAllText().Once()` fails with:

```text title="Failure message"
Expected that fileSystem.Statistics
has recorded a call to File.WriteAllText exactly once,
but it was recorded 2 times
```

The mirror has one entry per `IFileSystem` member (`.File`, `.Directory`, `.FileInfo[path]`, `.DirectoryInfo[path]`,
`.DriveInfo`, `.FileStream`, `.FileSystemWatcher`, `.FileVersionInfo`, `.Path`), with one method per underlying API and
an indexer (`[path]`) for the instances of a path. Every result supports the quantifiers `Once`, `Twice`, `Never`,
`Exactly`, `AtLeast`, `AtMost`, `Between`, … and verifies at least one call without a quantifier.

:::note
As for every quantifier in this package, only one can be specified: chaining two, e.g. `.Once().Twice()`, throws an
`InvalidOperationException`; use `Between(minimum).And(maximum)` for a range.
:::

Property reads and writes are recorded with `.Get()` and `.Set()`:

```csharp
fileSystem.FileInfo.New("yesterday.txt").IsReadOnly = true;

await That(fileSystem.Statistics).Recorded().FileInfo["yesterday.txt"].IsReadOnly.Set().Once();
await That(fileSystem.Statistics).Recorded().FileInfo["yesterday.txt"].IsReadOnly.Get().Never();
```

Each parameter of a mirror method is an optional `Func<T, bool>` predicate that is matched **by position** against the
recorded arguments.

<details>
<summary>How the predicates are matched by position</summary>

- Without a predicate (or with `null`) the position is skipped and every overload matches, so `.File.Open()` counts
  _all_ `Open` calls regardless of their arguments.
- A predicate at a position beyond the number of parameters of an overload excludes that overload, so filtering
  `recursive` on `Directory.Delete` only matches the overload with two parameters.
- A predicate whose type differs from the recorded type at that position excludes that overload, so filtering
  `searchOption` on `Directory.EnumerateDirectories` never matches the `EnumerationOptions` overload.

A few methods cannot be filtered completely by position, because two of their overloads have different types at the
same position (`File.Open` / `FileInfo.Open` with `FileStreamOptions`, `FileSystemWatcher.WaitForChanged` with
`TimeSpan`).

</details>
