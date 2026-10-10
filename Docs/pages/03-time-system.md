# Time system

You can verify the timers of a `MockTimeSystem`.

The samples use the `TimeSpan` helpers (`5.Seconds()`, `100.Milliseconds()`) from
[aweXpect.Chronology](https://github.com/Testably/aweXpect.Chronology).

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
