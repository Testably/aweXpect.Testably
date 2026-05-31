using System.Threading;
using aweXpect.Core;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.TimeSystem;

// ReSharper disable UseAwaitUsing

namespace aweXpect.Testably.Tests;

public sealed class Timer
{
	public sealed class Executed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAutoAdvanceTriggersExecutions_ShouldSucceedSynchronously()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExecutionCountReachesThreshold_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));
				sut.Wait(3, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInsufficientExecutions_ShouldFailAfterTimeout()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					Timeout.InfiniteTimeSpan,
					Timeout.InfiniteTimeSpan);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Within(TimeSpan.FromMilliseconds(100)).Exactly(3.Times());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             executed exactly 3 times within 0:00.100,
					             but it was not executed
					             """);
			}

			[Fact]
			public async Task WhenLiveExecutionsArriveDuringWithin_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					Timeout.InfiniteTimeSpan,
					Timeout.InfiniteTimeSpan);

				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					// ReSharper disable once AccessToDisposedClosure
					sut.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(5));
				});

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().AtLeast(2.Times()).Within(TimeSpan.FromSeconds(5));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegated_AndExecuted_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));
				sut.Wait(1, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).DoesNotComplyWith(it
						=> it.Executed().Within(TimeSpan.FromMilliseconds(100)));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             did not execute at least once within 0:00.100,
					             but it was executed *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNegatedWithAtLeastQuantifier_ShouldIncludeQuantifierInMessage()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));
				sut.Wait(3, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).DoesNotComplyWith(it
						=> it.Executed().AtLeast(3.Times()).Within(TimeSpan.FromMilliseconds(100)));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             did not execute at least 3 times within 0:00.100,
					             but it was executed *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNegatedWithNeverQuantifier_AndNotExecuted_ShouldFail()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					Timeout.InfiniteTimeSpan,
					Timeout.InfiniteTimeSpan);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).DoesNotComplyWith(it
						=> it.Executed().Never().Within(TimeSpan.FromMilliseconds(100)));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             executed at least once within 0:00.100,
					             but it was not executed
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ITimerMock? sut = null;

				async Task Act()
				{
					await That(sut!).Executed().Within(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             executed at least once within 0:00.010,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenNeverQuantifier_AndExecutedOnce_ShouldFailWithDidNotExecuteWording()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					Timeout.InfiniteTimeSpan);
				sut.Wait(1, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Never().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
						Expected that sut
						did not execute within 0:00.100,
						but it was executed once
						""");
			}

			[Fact]
			public async Task WhenExecutedOnce_AndExpectingExactlyTwice_ShouldRenderOnce()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					Timeout.InfiniteTimeSpan);
				sut.Wait(1, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Exactly(2.Times()).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
						Expected that sut
						executed exactly twice within 0:00.100,
						but it was executed once
						""");
			}

			[Fact]
			public async Task WhenExecutedTwice_AndExpectingExactlyThrice_ShouldRenderTwice()
			{
				MockTimeSystem timeSystem = new();
				ITimerMock? timer = null;
				int callbacks = 0;
				timer = (ITimerMock)timeSystem.Timer.New(
					_ =>
					{
						if (Interlocked.Increment(ref callbacks) >= 2)
						{
							// ReSharper disable once AccessToModifiedClosure
							timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
						}
					},
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(5));
				using ITimerMock sut = timer;
				sut.Wait(2, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Exactly(3.Times()).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
						Expected that sut
						executed exactly 3 times within 0:00.100,
						but it was executed twice
						""");
			}

			[Fact]
			public async Task WhenExecutedThreeTimes_AndExpectingMore_ShouldRenderTimes()
			{
				MockTimeSystem timeSystem = new();
				ITimerMock? timer = null;
				int callbacks = 0;
				timer = (ITimerMock)timeSystem.Timer.New(
					_ =>
					{
						if (Interlocked.Increment(ref callbacks) >= 3)
						{
							// ReSharper disable once AccessToModifiedClosure
							timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
						}
					},
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(5));
				using ITimerMock sut = timer;
				sut.Wait(3, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Exactly(4.Times()).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
						Expected that sut
						executed exactly 4 times within 0:00.100,
						but it was executed 3 times
						""");
			}

			[Fact]
			public async Task WithoutQuantifier_WhenExecutedAtLeastOnce_ShouldSucceed()
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.Zero,
					TimeSpan.FromMilliseconds(1));
				sut.Wait(1, 5000);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
