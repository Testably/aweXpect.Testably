using System.IO;
using aweXpect.Core;
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

				await That(Act).ThrowsException()
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

				await That(Act).ThrowsException()
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

				await That(Act).ThrowsException()
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

				await That(Act).ThrowsException()
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered a notification which has name equal to "other.txt" at least once within 0:00.100,
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

				await That(Act).ThrowsException()
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
					.WithParamName("expectation");
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

				await That(Act).ThrowsException()
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

				await That(Act).ThrowsException()
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
		}
	}
}
