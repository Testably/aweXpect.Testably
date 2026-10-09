using System.IO;
using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.TimeSystem;

// ReSharper disable UseAwaitUsing

namespace aweXpect.Testably.Tests;

public sealed class Readme
{
	public sealed class Tests
	{
		[Fact]
		public async Task DirectoryChain_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.Directory.CreateDirectory("foo/bar");
			fileSystem.Directory.CreateDirectory("foo/baz");
			fileSystem.File.WriteAllText("foo/bar/my-file.txt", "some content");

			await That(fileSystem).HasDirectory("foo").WithDirectories(d => d.HasCount().EqualTo(2));
			await That(fileSystem).HasDirectory("foo/bar").WithFiles(f => f
				.All().ComplyWith(x => x.HasContent("SOME CONTENT").IgnoringCase()));
		}

#if NET8_0_OR_GREATER
		[Fact]
		public async Task Drive_ShouldSucceed()
		{
			MockFileSystem fileSystem = new(o => o.SimulatingOperatingSystem(SimulationMode.Windows));
			fileSystem.WithDrive("D:", d => d.SetTotalSize(2048));

			IDriveInfo driveInfo = fileSystem.DriveInfo.New("D:");

			await That(driveInfo).HasAvailableFreeSpace(2048);
			await That(driveInfo).HasTotalSize(2048).And.HasTotalFreeSpace(2048);
			await That(driveInfo).HasDriveFormat("NTFS");
			await That(driveInfo).HasDriveType(DriveType.Fixed);
			await That(driveInfo).HasName(driveInfo.Name).And.HasVolumeLabel(driveInfo.VolumeLabel);
			await That(driveInfo).IsReady();
		}
#endif

		[Fact]
		public async Task FileChain_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("my-file.txt", "some content");

			await That(fileSystem).HasFile("my-file.txt").WithContent("some content").IgnoringCase();
			await That(fileSystem).HasFile("my-file.txt").WithContent().NotEqualTo("some unexpected content");
			await That(fileSystem).HasFile("my-file.txt").WithContent(Encoding.UTF8.GetBytes("some content"));

			fileSystem.File.WriteAllText("my-other-file.txt", "SOME CONTENT");
			fileSystem.File.WriteAllText("my-third-file.txt", "some other content");

			await That(fileSystem).HasFile("my-file.txt").WithContent().SameAs("my-other-file.txt").IgnoringCase();
			await That(fileSystem).HasFile("my-file.txt").WithContent().NotSameAs("my-third-file.txt");

			await That(fileSystem).HasFile("my-file.txt").WithCreationTime(DateTime.Now).Within(TimeSpan.FromSeconds(1));
			await That(fileSystem).HasFile("my-file.txt").WithLastAccessTime(DateTime.Now).Within(TimeSpan.FromSeconds(1));
			await That(fileSystem).HasFile("my-file.txt").WithLastWriteTime(DateTime.Now).Within(TimeSpan.FromSeconds(1));
		}

		[Fact]
		public async Task FileSystem_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.Directory.CreateDirectory("my/path");
			fileSystem.File.WriteAllText("my-file.txt", "some content");

			await That(fileSystem).HasDirectory("my/path");
			await That(fileSystem).HasFile("my-file.txt");

			await That(fileSystem).DoesNotHaveDirectory("not/here");
			await That(fileSystem).DoesNotHaveFile("missing.txt");
		}

		[Fact]
		public async Task FileVersionInfo_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.WithFileVersionInfo("*.dll", v => v
				.SetCompanyName("Acme")
				.SetProductName("Anvil")
				.SetFileVersion("1.2.3.4")
				.SetIsDebug(true));
			fileSystem.File.WriteAllText("Acme.dll", "");

			IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

			await That(info).HasCompanyName("Acme").And.HasProductName("Anvil");
			await That(info).HasFileVersion("1.2.3.4").And.HasFileMajorPart(1);
			await That(info).IsDebug().And.IsNotPreRelease();
		}

		[Fact]
		public async Task Notifications_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("my-file.txt", "some content");

			await That(fileSystem).TriggeredNotification();
			await That(fileSystem).TriggeredNotification(c => c.Name == "my-file.txt");

			_ = Task.Run(() => fileSystem.File.WriteAllText("foo.txt", "x"));
			await That(fileSystem).TriggeredNotification().Within(TimeSpan.FromMilliseconds(100));

			await That(fileSystem).DidNotTriggerNotification(c => c.Name == "secret.txt");
		}

		[Fact]
		public async Task NotificationsWithQuantifier_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("a.txt", "x");
			fileSystem.File.WriteAllText("b.txt", "y");

			await That(fileSystem).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
				.Exactly(2.Times());

			await That(fileSystem)
				.TriggeredNotification()
				.Which(c => c.HasName("a.txt").And.HasChangeType(WatcherChangeTypes.Created))
				.Exactly(1.Times());
		}

		[Fact]
		public async Task RecordedCalls_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("foo.txt", "x");

			await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
			await That(fileSystem.Statistics).Recorded().File.WriteAllText(path: p => p == "foo.txt").Once();

			fileSystem.FileInfo.New("foo.txt").IsReadOnly = true;

			await That(fileSystem.Statistics).Recorded().FileInfo["foo.txt"].IsReadOnly.Set().Once();
		}

		[Fact]
		public async Task Timer_ShouldSucceed()
		{
			MockTimeSystem timeSystem = new();
			using ITimerMock timer = (ITimerMock)timeSystem.Timer.New(
				_ => { }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));

			await That(timer).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(5));
			await That(timer).Executed().AtLeast(2.Times()).Within(TimeSpan.FromMilliseconds(100));
		}

		[Fact]
		public async Task Watcher_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.InitializeIn("/watched");
			using IFileSystemWatcher watcher = fileSystem.FileSystemWatcher.New("/watched");
			watcher.EnableRaisingEvents = true;
			fileSystem.File.WriteAllText("my-file.txt", "some content");

			await That(watcher).Triggered();
			await That(watcher).Triggered(c => c.Name == "my-file.txt");
			await That(watcher).DidNotTrigger(c => c.Name == "secret.txt");

			await That(watcher)
				.Triggered()
				.Which(c => c.HasName("my-file.txt").And.HasChangeType(WatcherChangeTypes.Created))
				.Exactly(1.Times());
		}
	}
}
