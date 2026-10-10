using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed class TriggeredNotification
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventArrivesAsynchronously_ShouldSucceedWithinTimeout()
			{
				MockFileSystem sut = new();
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					sut.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).TriggeredNotification().Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLiveEventDoesNotMatchPredicate_ShouldFailAfterTimeout()
			{
				MockFileSystem sut = new();
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					sut.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).TriggeredNotification(c => c.Name == "other.txt")
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification matching c => c.Name == "other.txt" at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenLiveEventDoesNotMatchWhich_ShouldFailAfterTimeout()
			{
				MockFileSystem sut = new();
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					sut.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.HasName("other.txt"))
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification which has name equal to "other.txt" at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenLiveEventMatchesPredicate_ShouldSucceedWithinTimeout()
			{
				MockFileSystem sut = new();
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					sut.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).TriggeredNotification(c => c.Name == "foo.txt")
						.Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLiveEventMatchesWhich_ShouldSucceedWithinTimeout()
			{
				MockFileSystem sut = new();
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					sut.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.HasName("foo.txt"))
						.Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoPriorEvent_ShouldFailAfterTimeout()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenPriorEventExists_ShouldSucceedSynchronously()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				MockFileSystem? sut = null;

				async Task Act()
				{
					await That(sut!).TriggeredNotification().Within(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification at least once within 0:00.010,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhichWithInnerExpectation_ComposesWithQuantifier()
			{
				MockFileSystem sut = new();
				using IAwaitableCallback<ChangeDescription> reg = sut.Notify.OnEvent(
					_ => { },
					c => c.ChangeType == WatcherChangeTypes.Created);
				sut.File.WriteAllText("a.txt", "x");
				sut.File.WriteAllText("b.txt", "x");
				_ = reg.Wait(2, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.HasChangeType(WatcherChangeTypes.Created))
						.Exactly(2.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenChangeDoesNotMatch_ShouldFail()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.HasName("other.txt"))
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification which has name equal to "other.txt" at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhichWithReason_WhenChangeDoesNotMatch_ShouldIncludeReason()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.HasName("other.txt").Because("REASON-R"))
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification which has name equal to "other.txt" at least once within 0:00.100, because REASON-R,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhichWithNegatedInnerExpectation_WhenNoChange_ShouldDescribeNegation()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.DoesNotComplyWith(x => x.HasChangeType(WatcherChangeTypes.Deleted)))
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification which does not have change type Deleted at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhichTwice_WhenChangeDoesNotMatch_ShouldJoinFiltersWithAnd()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.HasName("foo.txt"))
						.Which(c => c.HasChangeType(WatcherChangeTypes.Deleted))
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("*which has name equal to \"foo.txt\" and has change type Deleted*").AsWildcard();
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenChangeMatches_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.HasName("foo.txt").And.HasChangeType(WatcherChangeTypes.Created));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhichWithNullExpectation_ShouldThrowArgumentNullException()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification().Which(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation").And
					.WithMessage("The 'expectation' cannot be null.*").AsWildcard();
			}

			[Fact]
			public async Task WithNullPredicate_ShouldThrowArgumentNullException()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.*").AsWildcard();
			}

			[Fact]
			public async Task WithinTwice_ShouldThrowInvalidOperationException()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Within(TimeSpan.FromMilliseconds(10))
						.Within(TimeSpan.FromMilliseconds(20));
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.")
					.Because("each option can only be specified once in v3 instead of the last one silently winning");
			}

			[Fact]
			public async Task WithExactlyOnce_WhenTriggeredTwice_ShouldFail()
			{
				MockFileSystem sut = new();
				using IAwaitableCallback<ChangeDescription> reg = sut.Notify.OnEvent(
					_ => { },
					c => c.ChangeType == WatcherChangeTypes.Created);
				sut.File.WriteAllText("a.txt", "x");
				sut.File.WriteAllText("b.txt", "x");
				ChangeDescription[] created = reg.Wait(2, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage($$"""
					               Expected that sut
					               triggered a notification matching c => c.ChangeType == WatcherChangeTypes.Created exactly once within 0:00.100,
					               but it was triggered twice in [
					                 {{created[0]}},
					                 {{created[1]}}
					               ]
					               """);
			}

			[Fact]
			public async Task WithPredicate_NarrowsAssertion()
			{
				MockFileSystem sut = new();
				using IAwaitableCallback<ChangeDescription> reg = sut.Notify.OnEvent(
					_ => { },
					c => c.ChangeType == WatcherChangeTypes.Created);
				sut.File.WriteAllText("foo.txt", "x");
				_ = reg.Wait(1, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithPredicate_WhenPriorEventDoesNotMatch_ShouldFail()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification(c => c.Name == "other.txt")
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification matching c => c.Name == "other.txt" at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WithPredicate_WhenPriorEventMatches_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification(c => c.Name == "foo.txt");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExactlyOnce_WhenTriggeredThreeTimes_ShouldFailWithCountInMessage()
			{
				MockFileSystem sut = new();
				using IAwaitableCallback<ChangeDescription> reg = sut.Notify.OnEvent(
					_ => { },
					c => c.ChangeType == WatcherChangeTypes.Created);
				sut.File.WriteAllText("a.txt", "x");
				sut.File.WriteAllText("b.txt", "x");
				sut.File.WriteAllText("c.txt", "x");
				ChangeDescription[] created = reg.Wait(3, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage($$"""
					               Expected that sut
					               triggered a notification matching c => c.ChangeType == WatcherChangeTypes.Created exactly once within 0:00.100,
					               but it was triggered 3 times in [
					                 {{created[0]}},
					                 {{created[1]}},
					                 {{created[2]}}
					               ]
					               """);
			}

			[Fact]
			public async Task WithZeroTimeout_WhenNoPriorEvent_ShouldFailWithTimeoutMessage()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification().Within(TimeSpan.Zero);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification at least once within 0:00,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WithNegativeTimeout_ShouldThrowArgumentOutOfRangeException()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification().Within(TimeSpan.FromSeconds(-1));
				}

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.*").AsWildcard();
			}

			[Fact]
			public async Task WithInfiniteTimeout_WhenPriorEventExists_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification().Within(Timeout.InfiniteTimeSpan);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithoutWithin_WhenNoPriorEvent_ShouldFailWithoutWaiting()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification().WithTimeout(TimeSpan.FromSeconds(5));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification at least once,
					             but it was not triggered
					             """)
					.Because("without Within only the notifications triggered so far are checked");
			}

			[Fact]
			public async Task WhenTimeoutOfEvaluationEndsWithin_ShouldFailWithTheReceivedNotifications()
			{
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Within(TimeSpan.FromMilliseconds(200))
						.WithTimeout(TimeSpan.FromMilliseconds(200));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             triggered a notification at least once within 0:00.200,
					             but it was not triggered
					             """)
					.Because("a timeout of the evaluation at the end of Within lets the received notifications decide");
			}

			[Fact]
			public async Task WhenNegatedWithAtLeast_ShouldDescribeNegatedQuantifier()
			{
				MockFileSystem sut = new();
				using IAwaitableCallback<ChangeDescription> reg = sut.Notify.OnEvent(
					_ => { },
					c => c.ChangeType == WatcherChangeTypes.Created);
				sut.File.WriteAllText("a.txt", "x");
				sut.File.WriteAllText("b.txt", "x");
				ChangeDescription[] created = reg.Wait(2, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).DoesNotComplyWith(x => x
						.TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
						.AtLeast(2.Times())
						.Within(TimeSpan.FromMilliseconds(100)));
				}

				await That(Act).Throws()
					.WithMessage($$"""
					               Expected that sut
					               triggered a notification matching c => c.ChangeType == WatcherChangeTypes.Created fewer than twice within 0:00.100,
					               but it was triggered twice in [
					                 {{created[0]}},
					                 {{created[1]}}
					               ]
					               """);
			}

			[Fact]
			public async Task WhenNegatedWithTwice_ShouldDescribeNegatedQuantifier()
			{
				MockFileSystem sut = new();
				using IAwaitableCallback<ChangeDescription> reg = sut.Notify.OnEvent(
					_ => { },
					c => c.ChangeType == WatcherChangeTypes.Created);
				sut.File.WriteAllText("a.txt", "x");
				sut.File.WriteAllText("b.txt", "x");
				ChangeDescription[] created = reg.Wait(2, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).DoesNotComplyWith(x => x
						.TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
						.Twice()
						.Within(TimeSpan.FromMilliseconds(100)));
				}

				await That(Act).Throws()
					.WithMessage($$"""
					               Expected that sut
					               triggered a notification matching c => c.ChangeType == WatcherChangeTypes.Created not exactly twice within 0:00.100,
					               but it was triggered twice in [
					                 {{created[0]}},
					                 {{created[1]}}
					               ]
					               """);
			}

			[Fact]
			public async Task WithNever_WhenTriggered_ShouldFail()
			{
				MockFileSystem sut = new();
				ChangeDescription? firstEvent = null;
				using IAwaitableCallback<ChangeDescription> reg = sut.Notify.OnEvent(c => firstEvent ??= c);
				sut.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
						.Never()
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage($$"""
					               Expected that sut
					               did not trigger a notification matching c => c.ChangeType == WatcherChangeTypes.Created within 0:00.100,
					               but it was triggered once in [
					                 {{firstEvent}}
					               ]
					               """);
			}

			[Fact]
			public async Task WhenEvaluatedForMultipleFileSystems_ShouldCountEachFileSystemSeparately()
			{
				MockFileSystem[] fileSystems = [new(), new(),];
				foreach (MockFileSystem fileSystem in fileSystems)
				{
					fileSystem.File.WriteAllText("a.txt", "x");
					fileSystem.File.WriteAllText("b.txt", "x");
					fileSystem.File.WriteAllText("c.txt", "x");
				}

				async Task Act()
				{
					await That(fileSystems).All().ComplyWith(fs => fs
						.TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
						.Between(3).And(3.Times())
						.Within(TimeSpan.FromMilliseconds(100)));
				}

				await That(Act).DoesNotThrow()
					.Because("each file system triggered exactly three created notifications");
			}

			[Fact]
			public async Task WhenNotificationIsRaisedOnThreadWithSingleThreadedContext_ShouldNotBlockIt()
			{
				MockFileSystem sut = new();
				// Not disposed, as the background thread may still wait on it when the test fails.
				ManualResetEventSlim isEvaluated = new();
				Thread writer = new(() =>
				{
					QueuingSynchronizationContext context = new();
					SynchronizationContext.SetSynchronizationContext(context);
					// Repeated, as notifications raised before the expectation subscribed are not observed.
					do
					{
						sut.File.WriteAllText("foo.txt", "x");
						context.RunPending();
					} while (!isEvaluated.Wait(TimeSpan.FromMilliseconds(10)));
				})
				{
					IsBackground = true,
				};

				async Task Act()
				{
					writer.Start();
					try
					{
						await That(sut).TriggeredNotification()
							.Which(c => c.IsAcceptedAfter(async _ => await Task.Yield()))
							.Within(TimeSpan.FromSeconds(5));
					}
					finally
					{
						isEvaluated.Set();
					}
				}

				await That(Act).DoesNotThrow();
				await That(writer.Join(TimeSpan.FromSeconds(10))).IsTrue()
					.Because("the nested expectation must not be evaluated synchronously on the notifying thread");
			}

			[Fact]
			public async Task WhenNestedExpectationOutlastsTheTimeout_ShouldNotThrowIntoNotifyingCode()
			{
				MockFileSystem sut = new();
				Exception? writerException = null;
				Task writer = Task.Run(async () =>
				{
					await Task.Delay(20);
					try
					{
						sut.File.WriteAllText("foo.txt", "x");
					}
					catch (Exception exception)
					{
						writerException = exception;
					}
				});

				async Task Act()
				{
					await That(sut).TriggeredNotification()
						.Which(c => c.IsAcceptedAfter(token => Task.Delay(1000, token)))
						.Within(TimeSpan.FromMilliseconds(500));
				}

				await That(Act).DoesNotThrow()
					.Because("the notification was triggered within the timeout");
				await writer;
				await That(writerException).IsNull()
					.Because("the timeout of the expectation must not cancel the code that raised the notification");
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFailWithoutThrowingIntoNotifyingCode()
			{
				MockFileSystem sut = new();
				Exception? writerException = null;
				Task writer = Task.Run(async () =>
				{
					await Task.Delay(20);
					try
					{
						sut.File.WriteAllText("foo.txt", "x");
					}
					catch (Exception exception)
					{
						writerException = exception;
					}
				});

				async Task Act()
				{
					await That(sut).TriggeredNotification(_ => throw new InvalidOperationException("boom"))
						.Within(TimeSpan.FromSeconds(5));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that sut
					             triggered a notification matching _ => throw new InvalidOperationException("boom") at least once within 0:05,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """);
				await writer;
				await That(writerException).IsNull()
					.Because("an exception of the predicate must not be thrown into the code that raised the notification");
			}
		}
	}
}

file sealed class QueuingSynchronizationContext : SynchronizationContext
{
	private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _pending = new();

	public override void Post(SendOrPostCallback d, object? state)
		=> _pending.Enqueue((d, state));

	public void RunPending()
	{
		while (_pending.TryDequeue(out (SendOrPostCallback Callback, object? State) item))
		{
			item.Callback(item.State);
		}
	}
}

file static class AsyncChangeDescriptionExpectations
{
	public static AndOrResult<ChangeDescription, IThat<ChangeDescription>> IsAcceptedAfter(
		this IThat<ChangeDescription> subject, Func<CancellationToken, Task> wait)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsAcceptedAfterConstraint(it, grammars, wait)),
			subject);

	private sealed class IsAcceptedAfterConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<CancellationToken, Task> wait)
		: ConstraintResult.WithNotNullValue<ChangeDescription>(it, grammars),
			IAsyncConstraint<ChangeDescription>
	{
		public async ValueTask<ConstraintResult> IsMetBy(ChangeDescription actual,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			await wait(cancellationToken);
			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is accepted asynchronously");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not accepted asynchronously");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was");
	}
}
