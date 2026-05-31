using System.IO;
using System.IO.Abstractions;
using aweXpect.Core;
using Testably.Abstractions;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystemWatcher
{
	public sealed class Triggered
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAnotherWatcherFires_ShouldNotCountTowardThisWatcher()
			{
				MockFileSystem fs = new();
				fs.Directory.CreateDirectory("/a");
				fs.Directory.CreateDirectory("/b");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/a");
				using IFileSystemWatcher other = fs.FileSystemWatcher.New("/b");
				sut.EnableRaisingEvents = true;
				other.EnableRaisingEvents = true;
				fs.File.WriteAllText("/b/foo.txt", "x");

				async Task Act()
				{
					await That(sut).Triggered().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered an event at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenEventArrivesAsynchronously_ShouldSucceedWithinTimeout()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					fs.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).Triggered().Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLiveEventDoesNotMatchPredicate_ShouldFailAfterTimeout()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					fs.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).Triggered(c => c.Name == "other.txt")
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered an event matching c => c.Name == "other.txt" at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenLiveEventDoesNotMatchWhich_ShouldFailAfterTimeout()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					fs.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).Triggered()
						.Which(c => c.HasName("other.txt"))
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered an event which has name equal to "other.txt" at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenLiveEventMatchesPredicate_ShouldSucceedWithinTimeout()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					fs.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).Triggered(c => c.Name == "foo.txt")
						.Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLiveEventMatchesWhich_ShouldSucceedWithinTimeout()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				_ = Task.Run(async () =>
				{
					await Task.Delay(20);
					fs.File.WriteAllText("foo.txt", "x");
				});

				async Task Act()
				{
					await That(sut).Triggered()
						.Which(c => c.HasName("foo.txt"))
						.Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoEvent_ShouldFailAfterTimeout()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;

				async Task Act()
				{
					await That(sut).Triggered().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered an event at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhenPriorEventExists_ShouldSucceedSynchronously()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				fs.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).Triggered();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotFromMockFileSystem_ShouldThrowInvalidOperationException()
			{
				RealFileSystem realFs = new();
				using IFileSystemWatcher sut = realFs.FileSystemWatcher.New(Path.GetTempPath());

				async Task Act()
				{
					await That(sut).Triggered().Within(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).Throws<InvalidOperationException>();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IFileSystemWatcher? sut = null;

				async Task Act()
				{
					await That(sut!).Triggered().Within(TimeSpan.FromMilliseconds(10));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered an event at least once within 0:00.010,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhichWithInnerExpectation_ComposesWithQuantifier()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				using IAwaitableCallback<WatcherChangeDescription> reg = fs.Watcher.OnTriggered(
					_ => { },
					c => c.FileSystemWatcher == sut && c.ChangeType == WatcherChangeTypes.Created);
				fs.File.WriteAllText("a.txt", "x");
				fs.File.WriteAllText("b.txt", "x");
				_ = reg.Wait(2, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).Triggered()
						.Which(c => c.HasChangeType(WatcherChangeTypes.Created))
						.Exactly(2.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenChangeDoesNotMatch_ShouldFail()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				fs.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).Triggered()
						.Which(c => c.HasName("other.txt"))
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered an event which has name equal to "other.txt" at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenChangeMatches_ShouldSucceed()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				fs.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).Triggered()
						.Which(c => c.HasName("foo.txt").And.HasChangeType(WatcherChangeTypes.Created));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhichWithNullExpectation_ShouldThrowArgumentNullException()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;

				async Task Act()
				{
					await That(sut).Triggered().Which(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation");
			}

			[Fact]
			public async Task WithExactlyOnce_WhenTriggeredTwice_ShouldFail()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				using IAwaitableCallback<WatcherChangeDescription> reg = fs.Watcher.OnTriggered(
					_ => { },
					c => c.FileSystemWatcher == sut && c.ChangeType == WatcherChangeTypes.Created);
				fs.File.WriteAllText("a.txt", "x");
				fs.File.WriteAllText("b.txt", "x");
				WatcherChangeDescription[] created = reg.Wait(2, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).Triggered(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage($$"""
					               Expected that sut
					               triggered an event matching c => c.ChangeType == WatcherChangeTypes.Created exactly once within 0:00.100,
					               but it was triggered twice in [
					                 {{created[0]}},
					                 {{created[1]}}
					               ]
					               """);
			}

			[Fact]
			public async Task WithPredicate_NarrowsAssertion()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				using IAwaitableCallback<WatcherChangeDescription> reg = fs.Watcher.OnTriggered(
					_ => { },
					c => c.FileSystemWatcher == sut && c.ChangeType == WatcherChangeTypes.Created);
				fs.File.WriteAllText("foo.txt", "x");
				_ = reg.Wait(1, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).Triggered(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithPredicate_WhenPriorEventDoesNotMatch_ShouldFail()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				fs.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).Triggered(c => c.Name == "other.txt")
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered an event matching c => c.Name == "other.txt" at least once within 0:00.100,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WithPredicate_WhenPriorEventMatches_ShouldSucceed()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				fs.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).Triggered(c => c.Name == "foo.txt");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExactlyOnce_WhenTriggeredThreeTimes_ShouldFailWithCountInMessage()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				using IAwaitableCallback<WatcherChangeDescription> reg = fs.Watcher.OnTriggered(
					_ => { },
					c => c.FileSystemWatcher == sut && c.ChangeType == WatcherChangeTypes.Created);
				fs.File.WriteAllText("a.txt", "x");
				fs.File.WriteAllText("b.txt", "x");
				fs.File.WriteAllText("c.txt", "x");
				WatcherChangeDescription[] created = reg.Wait(3, TimeSpan.FromSeconds(30));

				async Task Act()
				{
					await That(sut).Triggered(c => c.ChangeType == WatcherChangeTypes.Created)
						.Exactly(1.Times())
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).ThrowsException()
					.WithMessage($$"""
					               Expected that sut
					               triggered an event matching c => c.ChangeType == WatcherChangeTypes.Created exactly once within 0:00.100,
					               but it was triggered 3 times in [
					                 {{created[0]}},
					                 {{created[1]}},
					                 {{created[2]}}
					               ]
					               """);
			}

			[Fact]
			public async Task WithZeroTimeout_WhenNoEvent_ShouldFailWithTimeoutMessage()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;

				async Task Act()
				{
					await That(sut).Triggered().Within(TimeSpan.Zero);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that sut
					             triggered an event at least once within 0:00,
					             but it was not triggered
					             """);
			}

			[Fact]
			public async Task WithNegativeTimeout_ShouldThrowArgumentOutOfRangeException()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;

				async Task Act()
				{
					await That(sut).Triggered().Within(TimeSpan.FromMilliseconds(-1));
				}

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.*").AsWildcard();
			}
		}
	}
}
