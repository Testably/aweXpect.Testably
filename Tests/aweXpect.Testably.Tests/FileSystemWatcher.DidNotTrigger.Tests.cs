using System.IO.Abstractions;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystemWatcher
{
	public sealed class DidNotTrigger
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAnotherWatcherFires_ShouldStillSucceed()
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
					await That(sut).DidNotTrigger().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventFired_ShouldFail()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				using IAwaitableCallback<WatcherChangeDescription> reg = fs.Watcher.OnTriggered(
					_ => { },
					c => c.FileSystemWatcher == sut);
				fs.File.WriteAllText("foo.txt", "x");
				WatcherChangeDescription firstEvent = reg.Wait(1, TimeSpan.FromSeconds(30))[0];

				async Task Act()
				{
					await That(sut).DidNotTrigger();
				}

				await That(Act).ThrowsException()
					.WithMessage($$"""
					               Expected that sut
					               did not trigger an event,
					               but it was triggered once in [
					                 {{firstEvent}}
					               ]
					               """);
			}

			[Fact]
			public async Task WhenNoEvent_ShouldSucceed()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;

				async Task Act()
				{
					await That(sut).DidNotTrigger().Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenMatchingChange_ShouldFail()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				using IAwaitableCallback<WatcherChangeDescription> reg = fs.Watcher.OnTriggered(
					_ => { },
					c => c.FileSystemWatcher == sut && c.Name == "foo.txt");
				fs.File.WriteAllText("foo.txt", "x");
				WatcherChangeDescription firstMatch = reg.Wait(1, TimeSpan.FromSeconds(30))[0];

				async Task Act()
				{
					await That(sut).DidNotTrigger()
						.Which(c => c.HasName("foo.txt"));
				}

				await That(Act).ThrowsException()
					.WithMessage($$"""
					               Expected that sut
					               did not trigger an event which has name equal to "foo.txt",
					               but it was triggered once in [
					                 {{firstMatch}}
					               ]
					               """);
			}

			[Fact]
			public async Task WhichWithInnerExpectation_WhenNoMatchingChange_ShouldSucceed()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				fs.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).DidNotTrigger()
						.Which(c => c.HasName("other.txt"))
						.Within(TimeSpan.FromMilliseconds(100));
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
					await That(sut).DidNotTrigger().Which(null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectation");
			}

			[Fact]
			public async Task WithPredicate_WhenMatchingEvent_ShouldFail()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				using IAwaitableCallback<WatcherChangeDescription> reg = fs.Watcher.OnTriggered(
					_ => { },
					c => c.FileSystemWatcher == sut && c.Name == "foo.txt");
				fs.File.WriteAllText("foo.txt", "x");
				WatcherChangeDescription firstMatch = reg.Wait(1, TimeSpan.FromSeconds(30))[0];

				async Task Act()
				{
					await That(sut).DidNotTrigger(c => c.Name == "foo.txt");
				}

				await That(Act).ThrowsException()
					.WithMessage($$"""
					               Expected that sut
					               did not trigger an event matching c => c.Name == "foo.txt",
					               but it was triggered once in [
					                 {{firstMatch}}
					               ]
					               """);
			}

			[Fact]
			public async Task WithPredicate_WhenNoMatchingEvent_ShouldSucceed()
			{
				MockFileSystem fs = new();
				fs.InitializeIn("/x");
				using IFileSystemWatcher sut = fs.FileSystemWatcher.New("/x");
				sut.EnableRaisingEvents = true;
				fs.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(sut).DidNotTrigger(c => c.Name == "other.txt")
						.Within(TimeSpan.FromMilliseconds(100));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventArrivesAsynchronouslyWithinTimeout_ShouldFail()
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
					await That(sut).DidNotTrigger().Within(TimeSpan.FromSeconds(30));
				}

				await That(Act).ThrowsException()
					.WithMessage("*did not trigger an event*but it was triggered*").AsWildcard();
			}
		}
	}
}
