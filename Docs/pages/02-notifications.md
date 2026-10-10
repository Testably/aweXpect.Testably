# Notifications and recorded calls

You can verify which changes the code under test made to a `MockFileSystem`: the notifications it raised, the events
of a single watcher, and the method calls and property accesses it recorded.

The samples use the `TimeSpan` helpers (`100.Milliseconds()`) from
[aweXpect.Chronology](https://github.com/Testably/aweXpect.Chronology).

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
