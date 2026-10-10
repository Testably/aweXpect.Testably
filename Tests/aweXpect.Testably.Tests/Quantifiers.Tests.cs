using System.IO;
using System.IO.Abstractions;
using System.Threading;
using aweXpect.Core;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;
using Testably.Abstractions.Testing.TimeSystem;

// ReSharper disable UseAwaitUsing

namespace aweXpect.Testably.Tests;

public sealed class Quantifiers
{
	public sealed class ChainedCountTests
	{
		[Fact]
		public async Task RecordedMethodCall_WhenCountsAreChained_ShouldThrowInvalidOperationException()
		{
			MockFileSystem fileSystem = new();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once().Twice();
			}

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Twice cannot be combined with Once.")
				.Because("each option can only be specified once in v3, so a later count no longer replaces an earlier one");
		}

		[Fact]
		public async Task TriggeredNotification_WhenCountsAreChained_ShouldThrowInvalidOperationException()
		{
			MockFileSystem sut = new();

			async Task Act()
			{
				await That(sut).TriggeredNotification().Once().Twice();
			}

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Twice cannot be combined with Once.")
				.Because("each option can only be specified once in v3, so a later count no longer replaces an earlier one");
		}
	}

	public sealed class TriggeredNotificationTests
	{
		[Fact]
		public async Task AtMost_WhenTriggeredMoreOften_ShouldFail()
		{
			MockFileSystem sut = new();
			ChangeDescription[] created = CreateTwoFiles(sut);

			async Task Act()
			{
				await That(sut).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
					.AtMost(1.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that sut
				               has triggered a notification matching c => c.ChangeType == WatcherChangeTypes.Created at most once within 0:00.100,
				               but it was triggered twice

				               Matching changes:
				               [
				                 {{created[0]}},
				                 {{created[1]}}
				               ]
				               """);
		}

		[Fact]
		public async Task Between_WhenTriggeredWithinRange_ShouldSucceed()
		{
			MockFileSystem sut = new();
			CreateTwoFiles(sut);

			async Task Act()
			{
				await That(sut).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
					.Between(1).And(3.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task LessThan_WhenTriggeredAsOften_ShouldFail()
		{
			MockFileSystem sut = new();
			ChangeDescription[] created = CreateTwoFiles(sut);

			async Task Act()
			{
				await That(sut).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
					.LessThan(2.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that sut
				               has triggered a notification matching c => c.ChangeType == WatcherChangeTypes.Created fewer than twice within 0:00.100,
				               but it was triggered twice

				               Matching changes:
				               [
				                 {{created[0]}},
				                 {{created[1]}}
				               ]
				               """);
		}

		[Fact]
		public async Task MoreThan_WhenTriggeredMoreOften_ShouldSucceed()
		{
			MockFileSystem sut = new();
			CreateTwoFiles(sut);

			async Task Act()
			{
				await That(sut).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
					.MoreThan(1.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).DoesNotThrow();
		}

		private static ChangeDescription[] CreateTwoFiles(MockFileSystem fileSystem)
		{
			using IAwaitableCallback<ChangeDescription> registration = fileSystem.Notify.OnEvent(
				_ => { },
				c => c.ChangeType == WatcherChangeTypes.Created);
			fileSystem.File.WriteAllText("a.txt", "x");
			fileSystem.File.WriteAllText("b.txt", "x");
			return registration.Wait(2, TimeSpan.FromSeconds(30));
		}
	}

	public sealed class TriggeredWatcherTests
	{
		[Fact]
		public async Task AtMost_WhenTriggeredMoreOften_ShouldFail()
		{
			MockFileSystem fileSystem = new();
			fileSystem.InitializeIn("/x");
			using IFileSystemWatcher sut = fileSystem.FileSystemWatcher.New("/x");
			sut.EnableRaisingEvents = true;
			WatcherChangeDescription[] created = CreateTwoFiles(fileSystem, sut);

			async Task Act()
			{
				// ReSharper disable once AccessToDisposedClosure
				await That(sut).Triggered(c => c.ChangeType == WatcherChangeTypes.Created)
					.AtMost(1.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that sut
				               has triggered an event matching c => c.ChangeType == WatcherChangeTypes.Created at most once within 0:00.100,
				               but it was triggered twice

				               Matching changes:
				               [
				                 {{created[0]}},
				                 {{created[1]}}
				               ]
				               """);
		}

		[Fact]
		public async Task Between_WhenTriggeredWithinRange_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.InitializeIn("/x");
			using IFileSystemWatcher sut = fileSystem.FileSystemWatcher.New("/x");
			sut.EnableRaisingEvents = true;
			CreateTwoFiles(fileSystem, sut);

			async Task Act()
			{
				// ReSharper disable once AccessToDisposedClosure
				await That(sut).Triggered(c => c.ChangeType == WatcherChangeTypes.Created)
					.Between(1).And(3.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task LessThan_WhenTriggeredAsOften_ShouldFail()
		{
			MockFileSystem fileSystem = new();
			fileSystem.InitializeIn("/x");
			using IFileSystemWatcher sut = fileSystem.FileSystemWatcher.New("/x");
			sut.EnableRaisingEvents = true;
			WatcherChangeDescription[] created = CreateTwoFiles(fileSystem, sut);

			async Task Act()
			{
				// ReSharper disable once AccessToDisposedClosure
				await That(sut).Triggered(c => c.ChangeType == WatcherChangeTypes.Created)
					.LessThan(2.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that sut
				               has triggered an event matching c => c.ChangeType == WatcherChangeTypes.Created fewer than twice within 0:00.100,
				               but it was triggered twice

				               Matching changes:
				               [
				                 {{created[0]}},
				                 {{created[1]}}
				               ]
				               """);
		}

		[Fact]
		public async Task MoreThan_WhenTriggeredMoreOften_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.InitializeIn("/x");
			using IFileSystemWatcher sut = fileSystem.FileSystemWatcher.New("/x");
			sut.EnableRaisingEvents = true;
			CreateTwoFiles(fileSystem, sut);

			async Task Act()
			{
				// ReSharper disable once AccessToDisposedClosure
				await That(sut).Triggered(c => c.ChangeType == WatcherChangeTypes.Created)
					.MoreThan(1.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).DoesNotThrow();
		}

		private static WatcherChangeDescription[] CreateTwoFiles(MockFileSystem fileSystem,
			IFileSystemWatcher watcher)
		{
			using IAwaitableCallback<WatcherChangeDescription> registration = fileSystem.Watcher.OnTriggered(
				_ => { },
				c => c.FileSystemWatcher == watcher && c.ChangeType == WatcherChangeTypes.Created);
			fileSystem.File.WriteAllText("a.txt", "x");
			fileSystem.File.WriteAllText("b.txt", "x");
			return registration.Wait(2, TimeSpan.FromSeconds(30));
		}
	}

	public sealed class TimerExecutedTests
	{
		[Fact]
		public async Task AtMost_WhenNotExecuted_ShouldSucceed()
		{
			MockTimeSystem timeSystem = new();
			using ITimerMock sut = CreateIdleTimer(timeSystem);

			async Task Act()
			{
				// ReSharper disable once AccessToDisposedClosure
				await That(sut).Executed().AtMost(1.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Between_WhenNotExecuted_ShouldFail()
		{
			MockTimeSystem timeSystem = new();
			using ITimerMock sut = CreateIdleTimer(timeSystem);

			async Task Act()
			{
				// ReSharper disable once AccessToDisposedClosure
				await That(sut).Executed().Between(1).And(3.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             has executed between 1 and 3 times within 0:00.100,
				             but it was not executed
				             """);
		}

		[Fact]
		public async Task LessThan_WhenNotExecuted_ShouldSucceed()
		{
			MockTimeSystem timeSystem = new();
			using ITimerMock sut = CreateIdleTimer(timeSystem);

			async Task Act()
			{
				// ReSharper disable once AccessToDisposedClosure
				await That(sut).Executed().LessThan(2.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task MoreThan_WhenNotExecuted_ShouldFail()
		{
			MockTimeSystem timeSystem = new();
			using ITimerMock sut = CreateIdleTimer(timeSystem);

			async Task Act()
			{
				// ReSharper disable once AccessToDisposedClosure
				await That(sut).Executed().MoreThan(1.Times()).Within(TimeSpan.FromMilliseconds(100));
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             has executed more than once within 0:00.100,
				             but it was not executed
				             """);
		}

		private static ITimerMock CreateIdleTimer(MockTimeSystem timeSystem)
			=> (ITimerMock)timeSystem.Timer.New(
				_ => { },
				null,
				Timeout.InfiniteTimeSpan,
				Timeout.InfiniteTimeSpan);
	}

	public sealed class RecordedMethodCallTests
	{
		[Fact]
		public async Task AtMost_WhenCalledMoreOften_ShouldFail()
		{
			MockFileSystem fileSystem = CreateWithTwoWrites();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded().File.WriteAllText().AtMost(1.Times());
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that fileSystem.Statistics
				             has recorded a call to File.WriteAllText at most once,
				             but it was recorded 2 times
				             """);
		}

		[Fact]
		public async Task Between_WhenCalledWithinRange_ShouldSucceed()
		{
			MockFileSystem fileSystem = CreateWithTwoWrites();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded().File.WriteAllText().Between(1).And(3.Times());
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task LessThan_WhenCalledAsOften_ShouldFail()
		{
			MockFileSystem fileSystem = CreateWithTwoWrites();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded().File.WriteAllText().LessThan(2.Times());
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that fileSystem.Statistics
				             has recorded a call to File.WriteAllText fewer than twice,
				             but it was recorded 2 times
				             """);
		}

		[Fact]
		public async Task MoreThan_WhenCalledMoreOften_ShouldSucceed()
		{
			MockFileSystem fileSystem = CreateWithTwoWrites();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded().File.WriteAllText().MoreThan(1.Times());
			}

			await That(Act).DoesNotThrow();
		}

		private static MockFileSystem CreateWithTwoWrites()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("a.txt", "x");
			fileSystem.File.WriteAllText("b.txt", "x");
			return fileSystem;
		}
	}

	public sealed class RecordedPropertyAccessTests
	{
		[Fact]
		public async Task AtMost_WhenAccessedMoreOften_ShouldFail()
		{
			MockFileSystem fileSystem = CreateWithTwoReads();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded()
					.FileInfo["foo.txt"].IsReadOnly.Get().AtMost(1.Times());
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that fileSystem.Statistics
				             has recorded a get of FileInfo["foo.txt"].IsReadOnly at most once,
				             but it was recorded 2 times
				             """);
		}

		[Fact]
		public async Task Between_WhenAccessedWithinRange_ShouldSucceed()
		{
			MockFileSystem fileSystem = CreateWithTwoReads();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded()
					.FileInfo["foo.txt"].IsReadOnly.Get().Between(1).And(3.Times());
			}

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task LessThan_WhenAccessedAsOften_ShouldFail()
		{
			MockFileSystem fileSystem = CreateWithTwoReads();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded()
					.FileInfo["foo.txt"].IsReadOnly.Get().LessThan(2.Times());
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that fileSystem.Statistics
				             has recorded a get of FileInfo["foo.txt"].IsReadOnly fewer than twice,
				             but it was recorded 2 times
				             """);
		}

		[Fact]
		public async Task MoreThan_WhenAccessedMoreOften_ShouldSucceed()
		{
			MockFileSystem fileSystem = CreateWithTwoReads();

			async Task Act()
			{
				await That(fileSystem.Statistics).Recorded()
					.FileInfo["foo.txt"].IsReadOnly.Get().MoreThan(1.Times());
			}

			await That(Act).DoesNotThrow();
		}

		private static MockFileSystem CreateWithTwoReads()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("foo.txt", "");
			IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");
			_ = fileInfo.IsReadOnly;
			_ = fileInfo.IsReadOnly;
			return fileSystem;
		}
	}
}
