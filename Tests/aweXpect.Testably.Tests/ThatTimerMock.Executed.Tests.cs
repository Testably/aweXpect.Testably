using System.Threading;
using aweXpect.Core;
using aweXpect.Core.Internal;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.TimeSystem;

// ReSharper disable UseAwaitUsing

namespace aweXpect.Testably.Tests;

public sealed class ThatTimerMock
{
	public sealed class Executed
	{
		public sealed class Tests
		{
			[Test]
			public async Task CheckEvery_WithZeroInterval_ShouldThrowArgumentOutOfRangeException()
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
					await That(sut).Executed().Within(TimeSpan.FromSeconds(1)).CheckEvery(TimeSpan.Zero);
				}

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("interval").And
					.WithMessage("The interval must be positive.*").AsWildcard();
			}

			[Test]
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

			[Test]
			public async Task WhenExecutedOnce_AndExpectingExactlyTwice_ShouldRenderOnce()
			{
				MockTimeSystem timeSystem = new(o => o.DisableAutoAdvance());
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.FromSeconds(1),
					Timeout.InfiniteTimeSpan);
				await That(sut).Executed().Never().Within(TimeSpan.FromMilliseconds(200))
					.Because("this gives the timer thread the time to start and to wait for its first due time before the mock clock is advanced");
				timeSystem.TimeProvider.AdvanceBy(TimeSpan.FromSeconds(2));
				await That(sut).Executed().AtLeast(1.Times()).Within(TimeSpan.FromSeconds(10));
				await That(sut.ExecutionCount).IsEqualTo(1L);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Exactly(2.Times()).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
						Expected that sut
						has executed exactly twice within 0:00.100,
						but it was executed once
						""");
			}

			[Test]
			public async Task WhenExecutedThreeTimes_AndExpectingMore_ShouldRenderTimes()
			{
				MockTimeSystem timeSystem = new(o => o.DisableAutoAdvance());
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.FromSeconds(1),
					TimeSpan.FromSeconds(1));
				await That(sut).Executed().Never().Within(TimeSpan.FromMilliseconds(200))
					.Because("this gives the timer thread the time to start and to wait for its first due time before the mock clock is advanced");
				timeSystem.TimeProvider.AdvanceBy(TimeSpan.FromMilliseconds(3500));
				await That(sut).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(10));
				await That(sut.ExecutionCount).IsEqualTo(3L);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Exactly(4.Times()).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
						Expected that sut
						has executed exactly 4 times within 0:00.100,
						but it was executed 3 times
						""");
			}

			[Test]
			public async Task WhenExecutedTwice_AndExpectingExactlyThrice_ShouldRenderTwice()
			{
				MockTimeSystem timeSystem = new(o => o.DisableAutoAdvance());
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.FromSeconds(1),
					TimeSpan.FromSeconds(1));
				await That(sut).Executed().Never().Within(TimeSpan.FromMilliseconds(200))
					.Because("this gives the timer thread the time to start and to wait for its first due time before the mock clock is advanced");
				timeSystem.TimeProvider.AdvanceBy(TimeSpan.FromMilliseconds(2500));
				await That(sut).Executed().AtLeast(2.Times()).Within(TimeSpan.FromSeconds(10));
				await That(sut.ExecutionCount).IsEqualTo(2L);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Exactly(3.Times()).Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
						Expected that sut
						has executed exactly 3 times within 0:00.100,
						but it was executed twice
						""");
			}

			[Test]
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

			[Test]
			public async Task WhenExecutionsArriveLater_WithCheckEvery_ShouldSucceed()
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
					await That(sut).Executed().AtLeast(2.Times())
						.Within(TimeSpan.FromSeconds(5))
						.CheckEvery(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
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
					await That(sut).Executed().Within(TimeSpan.FromMilliseconds(100)).Exactly(3.Times())
						.WithTimeSystem(new VirtualTimeSystem());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             has executed exactly 3 times within 0:00.100,
					             but it was not executed
					             """);
			}

			[Test]
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

			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             has not executed at least once within 0:00.100,
					             but it was executed *
					             """).AsWildcard();
			}

			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             has not executed at least 3 times within 0:00.100,
					             but it was executed *
					             """).AsWildcard();
			}

			[Test]
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
						=> it.Executed().Never().Within(TimeSpan.FromMilliseconds(100)))
						.WithTimeSystem(new VirtualTimeSystem());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             has executed at least once within 0:00.100,
					             but it was not executed
					             """);
			}

			[Test]
			public async Task WhenNeverAndTimeoutOfEvaluationEndsWithin_ShouldSucceed()
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
					await That(sut).Executed().Never()
						.Within(TimeSpan.FromMilliseconds(200))
						.WithTimeout(TimeSpan.FromMilliseconds(200))
						.WithTimeSystem(new VirtualTimeSystem());
				}

				await That(Act).DoesNotThrow()
					.Because("a timeout of the evaluation at the end of Within lets the last check decide");
			}

			[Test]
			public async Task WhenNeverQuantifier_AndExecutedOnce_ShouldFailWithDidNotExecuteWording()
			{
				MockTimeSystem timeSystem = new(o => o.DisableAutoAdvance());
				using ITimerMock sut = (ITimerMock)timeSystem.Timer.New(
					_ => { },
					null,
					TimeSpan.FromSeconds(1),
					Timeout.InfiniteTimeSpan);
				await That(sut).Executed().Never().Within(TimeSpan.FromMilliseconds(200))
					.Because("this gives the timer thread the time to start and to wait for its first due time before the mock clock is advanced");
				timeSystem.TimeProvider.AdvanceBy(TimeSpan.FromSeconds(2));
				await That(sut).Executed().AtLeast(1.Times()).Within(TimeSpan.FromSeconds(10));
				await That(sut.ExecutionCount).IsEqualTo(1L);

				async Task Act()
				{
					// ReSharper disable once AccessToDisposedClosure
					await That(sut).Executed().Never().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).Throws()
					.WithMessage("""
						Expected that sut
						has never executed within 0:00.100,
						but it was executed once
						""");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ITimerMock? sut = null;

				async Task Act()
				{
					await That(sut!).Executed().Within(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             has executed at least once within 0:00.010,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithinTwice_ShouldUseLastTimeout()
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
					await That(sut).Executed()
						.Within(TimeSpan.FromMilliseconds(10))
						.Within(TimeSpan.FromMilliseconds(20))
						.WithTimeSystem(new VirtualTimeSystem());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             has executed at least once within 0:00.020,
					             but it was not executed
					             """)
					.Because("a repeated option replaces the earlier one since aweXpect v4");
			}

			[Test]
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

			[Test]
			public async Task WithoutWithin_WhenNeverAndNotExecuted_ShouldSucceedWithoutWaiting()
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
					await That(sut).Executed().Never().WithTimeout(TimeSpan.FromSeconds(5));
				}

				await That(Act).DoesNotThrow()
					.Because("without Within the execution count is checked once");
			}

			[Test]
			public async Task WithoutWithin_WhenNotExecuted_ShouldFailWithoutWaiting()
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
					await That(sut).Executed().WithTimeout(TimeSpan.FromSeconds(5));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that sut
					             has executed at least once,
					             but it was not executed
					             """)
					.Because("without Within the execution count is checked once");
			}
		}
	}
}
