using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.TimeSystem;
using static aweXpect.Expect;

namespace aweXpect.Testably.Aot;

internal static class Checks
{
	private static readonly TimeSpan ShortTimeout = TimeSpan.FromMilliseconds(100);

	public static readonly Check[] All =
	[
		new("an existing file passes",
			() => ShouldPass(async () => await That(CreateFileSystem()).HasFile("a.txt"))),
		new("a missing file fails",
			() => ShouldFail(async () => await That(CreateFileSystem()).HasFile("missing.txt"),
				"has file 'missing.txt'", "but it did not exist")),
		new("an existing directory passes",
			() => ShouldPass(async () => await That(CreateFileSystem()).HasDirectory("dir"))),
		new("a file instead of a directory fails",
			() => ShouldFail(async () => await That(CreateFileSystem()).HasDirectory("a.txt"),
				"has directory 'a.txt'", "but it was a file")),
		new("an existing drive passes",
			() => ShouldPass(async () =>
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				fileSystem.WithDrive("D:");
				await That(fileSystem).HasDrive("D:\\");
			})),
		new("a missing drive fails",
			() => ShouldFail(async () =>
			{
				MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
				await That(fileSystem).HasDrive("Z:\\");
			}, "has drive 'Z:\\'", "but it did not exist")),
		new("an equal string content passes",
			() => ShouldPass(async () => await That(CreateFileSystem()).HasFile("a.txt")
				.WithContent("HELLO").IgnoringCase())),
		new("a differing string content fails",
			() => ShouldFail(async () => await That(CreateFileSystem()).HasFile("a.txt").WithContent("help"),
				"with content equal to \"help\"", "but it was \"hello\", which differs at index 3")),
		new("an equal binary content passes",
			() => ShouldPass(async () => await That(CreateFileSystem()).HasFile("a.txt")
				.WithContent(Encoding.UTF8.GetBytes("hello")))),
		new("a differing binary content fails",
			() => ShouldFail(async () => await That(CreateFileSystem()).HasFile("a.txt").WithContent([1, 2, 3,]),
				"has file 'a.txt' with content equal to", "but it differed")),
		new("an expectation on the file bridged with Which passes",
			() => ShouldPass(async () => await That(CreateFileSystem()).HasFile("a.txt")
				.Which.HasLength(5).And.HasContent("hello"))),
		new("a failing expectation on the file bridged with Which fails",
			() => ShouldFail(async () => await That(CreateFileSystem()).HasFile("a.txt").Which.HasLength(99),
				"has file 'a.txt' which has length 99", "but it was 5")),
		new("an equal last write time passes",
			() => ShouldPass(async () =>
			{
				MockFileSystem fileSystem = CreateFileSystem();
				DateTime lastWriteTime = fileSystem.File.GetLastWriteTime("a.txt");
				await That(fileSystem).HasFile("a.txt").WithLastWriteTime(lastWriteTime);
			})),
		new("a differing last write time fails",
			() => ShouldFail(async () =>
			{
				MockFileSystem fileSystem = CreateFileSystem();
				DateTime lastWriteTime = fileSystem.File.GetLastWriteTime("a.txt");
				await That(fileSystem).HasFile("a.txt").WithLastWriteTime(lastWriteTime.AddDays(1));
			}, "has file 'a.txt' with last write time equal to", "but it was")),
		new("a triggered notification passes",
			() => ShouldPass(async () => await That(CreateFileSystem())
				.TriggeredNotification(c => c.Name == "a.txt"))),
		new("a missing notification fails",
			() => ShouldFail(async () => await That(CreateFileSystem())
					.TriggeredNotification(c => c.Name == "other.txt").Within(ShortTimeout),
				"triggered a notification matching c => c.Name == \"other.txt\" at least once within 0:00.100",
				"but it was not triggered")),
		new("a triggered watcher passes",
			() => ShouldPass(async () =>
			{
				MockFileSystem fileSystem = new();
				fileSystem.InitializeIn("/watched");
				using IFileSystemWatcher watcher = fileSystem.FileSystemWatcher.New("/watched");
				watcher.EnableRaisingEvents = true;
				fileSystem.File.WriteAllText("a.txt", "hello");
				await That(watcher).Triggered()
					.Which(c => c.HasName("a.txt").And.HasChangeType(WatcherChangeTypes.Created))
					.Exactly(1.Times())
					.Within(ShortTimeout);
			})),
		new("a watcher that was not triggered fails",
			() => ShouldFail(async () =>
			{
				MockFileSystem fileSystem = new();
				fileSystem.InitializeIn("/watched");
				using IFileSystemWatcher watcher = fileSystem.FileSystemWatcher.New("/watched");
				watcher.EnableRaisingEvents = true;
				await That(watcher).Triggered().Within(ShortTimeout);
			}, "triggered an event at least once within 0:00.100", "but it was not triggered")),
		new("an executed timer passes",
			() => ShouldPass(async () =>
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock timer = (ITimerMock)timeSystem.Timer.New(
					_ => { }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
				await That(timer).Executed().AtLeast(2.Times()).Within(TimeSpan.FromSeconds(5));
			})),
		new("a timer that was not executed fails",
			() => ShouldFail(async () =>
			{
				MockTimeSystem timeSystem = new();
				using ITimerMock timer = (ITimerMock)timeSystem.Timer.New(
					_ => { }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
				await That(timer).Executed().Within(ShortTimeout).Exactly(3.Times());
			}, "executed exactly 3 times within 0:00.100", "but it was not executed")),
		new("a recorded call passes",
			() => ShouldPass(async () => await That(CreateFileSystem().Statistics).Recorded()
				.File.WriteAllText(path: p => p == "a.txt").Once())),
		new("a recorded call whose predicate throws fails",
			() => ShouldFail(async () => await That(CreateFileSystem().Statistics).Recorded()
					.File.WriteAllText(path: _ => throw new InvalidOperationException("boom")).Never(),
				"but the path predicate did throw an InvalidOperationException", "boom")),
		new("a call that was not recorded fails",
			() => ShouldFail(async () => await That(CreateFileSystem().Statistics).Recorded()
					.File.Delete().Once(),
				"recorded a call to File.Delete exactly once", "but it was recorded 0 times")),
		new("a recorded property access passes",
			() => ShouldPass(async () =>
			{
				MockFileSystem fileSystem = CreateFileSystem();
				fileSystem.FileInfo.New("a.txt").IsReadOnly = true;
				await That(fileSystem.Statistics).Recorded().FileInfo["a.txt"].IsReadOnly.Set().Once();
			})),
		new("a property access that was not recorded fails",
			() => ShouldFail(async () => await That(CreateFileSystem().Statistics).Recorded()
					.FileInfo["a.txt"].IsReadOnly.Set().Once(),
				"set of FileInfo[\"a.txt\"].IsReadOnly", "but it was recorded 0 times")),
	];

	private static MockFileSystem CreateFileSystem()
	{
		MockFileSystem fileSystem = new();
		fileSystem.Directory.CreateDirectory("dir");
		fileSystem.File.WriteAllText("a.txt", "hello");
		return fileSystem;
	}

	private static async Task<string?> ShouldPass(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}

	/// <summary>
	///     Expects the failure of an expectation whose message contains every part.
	/// </summary>
	private static async Task<string?> ShouldFail(Func<Task> act, params string[] parts)
	{
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			string? missing = Array.Find(parts, part => !exception.Message.Contains(part, StringComparison.Ordinal));
			return missing is null ? null : $"message lacks \"{missing}\": {exception.Message}";
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} instead of {typeof(FailException).FullName}: {exception.Message}";
		}

		return "did not throw";
	}
}
