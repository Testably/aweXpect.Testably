# Time system

Describes the possible expectations for the timers of a `MockTimeSystem`.

| Expectation          | Negated    | Summary                         |
|----------------------|------------|---------------------------------|
| [`Executed`](#timer) | `.Never()` | the timer callback was executed |

The samples on this page use the following timer:

```csharp
using aweXpect.Chronology; // from the aweXpect.Chronology package

MockTimeSystem timeSystem = new();
ITimerMock timer = (ITimerMock)timeSystem.Timer.New(_ => { }, null, TimeSpan.Zero, 10.Milliseconds());
```

## Timer

A `MockTimeSystem` creates timers as `ITimerMock`. You can verify how often the timer callback was executed without
blocking the test thread:

```csharp
await Expect.That(timer).Executed().AtLeast(3.Times()).Within(5.Seconds());
```

`Executed()` accepts a quantifier (`AtLeast`, `AtMost`, `Exactly`, `Between`, `Never`, `Once`; at least once by
default). Without `.Within(timeout)`, it checks `ITimerMock.ExecutionCount` once and does not wait. With
`.Within(timeout)`, it checks the count again until the quantifier is decided or the timeout expires, so an upper bound
like `.Never()` or `.AtMost(…)` waits for the full timeout. Like for
[a condition](https://docs.testably.org/aweXpect/how-it-works/time-and-cancellation#a-condition), the count is checked
every [`DefaultCheckInterval`](https://docs.testably.org/aweXpect/how-it-works/configuration#settings), or in the
interval of `.CheckEvery(interval)`:

```csharp
await Expect.That(timer).Executed().AtLeast(3.Times()).Within(5.Seconds()).CheckEvery(10.Milliseconds());
```

A timeout of the evaluation (e.g. `.WithTimeout(…)`) that is not shorter than `.Within(timeout)` lets the last check
decide; a cancellation before that leaves the expectation
[inconclusive](https://docs.testably.org/aweXpect/how-it-works/time-and-cancellation#outcome).

For example, for a timer that never fires, `Expect.That(timer).Executed().AtLeast(3.Times()).Within(100.Milliseconds())`
fails with:

```text title="Failure message"
Expected that timer
has executed at least 3 times within 0:00.100,
but it was not executed
```
