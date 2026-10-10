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
			fileSystem.Directory.CreateDirectory("beatles/abbey-road");
			fileSystem.Directory.CreateDirectory("beatles/revolver");
			fileSystem.File.WriteAllText("beatles/abbey-road/something.txt", "something");

			await That(fileSystem).HasDirectory("beatles").WithDirectories(d => d.HasCount().EqualTo(2));
			await That(fileSystem).HasDirectory("beatles/abbey-road").WithFiles(f => f
				.All().ComplyWith(x => x.HasContent("SOMETHING").IgnoringCase()));
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
			fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

			await That(fileSystem).HasFile("let-it-be.txt").WithContent("LET IT BE").IgnoringCase();
			await That(fileSystem).HasFile("let-it-be.txt").WithContent().NotEqualTo("let it go");
			await That(fileSystem).HasFile("let-it-be.txt").WithContent(Encoding.UTF8.GetBytes("let it be"));

			fileSystem.File.WriteAllText("let-it-be-remastered.txt", "LET IT BE");
			fileSystem.File.WriteAllText("hey-jude.txt", "hey jude");

			await That(fileSystem).HasFile("let-it-be.txt").WithContent().SameAs("let-it-be-remastered.txt").IgnoringCase();
			await That(fileSystem).HasFile("let-it-be.txt").WithContent().NotSameAs("hey-jude.txt");

			await That(fileSystem).HasFile("let-it-be.txt").WithCreationTime(DateTime.Now).Within(TimeSpan.FromSeconds(1));
			await That(fileSystem).HasFile("let-it-be.txt").WithLastAccessTime(DateTime.Now).Within(TimeSpan.FromSeconds(1));
			await That(fileSystem).HasFile("let-it-be.txt").WithLastWriteTime(DateTime.Now).Within(TimeSpan.FromSeconds(1));
		}

		[Fact]
		public async Task FileSystem_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.Directory.CreateDirectory("beatles/abbey-road");
			fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

			await That(fileSystem).HasDirectory("beatles/abbey-road");
			await That(fileSystem).HasFile("let-it-be.txt");

			await That(fileSystem).DoesNotHaveDirectory("beatles/white-album");
			await That(fileSystem).DoesNotHaveFile("yesterday.txt");
		}

		[Fact]
		public async Task FileVersionInfo_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.WithFileVersionInfo("*.dll", v => v
				.SetCompanyName("Apple Corps")
				.SetProductName("Abbey Road")
				.SetFileVersion("1.9.6.9")
				.SetIsDebug(true));
			fileSystem.File.WriteAllText("AbbeyRoad.dll", "");

			IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("AbbeyRoad.dll");

			await That(info).HasCompanyName("Apple Corps").And.HasProductName("Abbey Road");
			await That(info).HasFileVersion("1.9.6.9").And.HasFileMajorPart(1);
			await That(info).IsDebug().And.IsNotPreRelease();
		}

		[Fact]
		public async Task Notifications_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

			await That(fileSystem).TriggeredNotification();
			await That(fileSystem).TriggeredNotification(c => c.Name == "let-it-be.txt");

			_ = Task.Run(() => fileSystem.File.WriteAllText("help.txt", "help"));
			await That(fileSystem).TriggeredNotification().Within(TimeSpan.FromMilliseconds(100));

			await That(fileSystem).DidNotTriggerNotification(c => c.Name == "unreleased.txt");
		}

		[Fact]
		public async Task NotificationsWithQuantifier_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("come-together.txt", "come together");
			fileSystem.File.WriteAllText("something.txt", "something");

			await That(fileSystem).TriggeredNotification(c => c.ChangeType == WatcherChangeTypes.Created)
				.Exactly(2.Times());

			await That(fileSystem)
				.TriggeredNotification()
				.Matching(c => c.HasName("come-together.txt").And.HasChangeType(WatcherChangeTypes.Created))
				.Exactly(1.Times());
		}

		[Fact]
		public async Task RecordedCalls_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("help.txt", "help");

			await That(fileSystem.Statistics).Recorded().File.WriteAllText().Once();
			await That(fileSystem.Statistics).Recorded().File.WriteAllText(path: p => p == "help.txt").Once();

			fileSystem.FileInfo.New("help.txt").IsReadOnly = true;

			await That(fileSystem.Statistics).Recorded().FileInfo["help.txt"].IsReadOnly.Set().Once();
		}

		[Fact]
		public async Task Timer_ShouldSucceed()
		{
			MockTimeSystem timeSystem = new();
			using ITimerMock timer = (ITimerMock)timeSystem.Timer.New(
				_ => { }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));

			await That(timer).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(5));
			await That(timer).Executed().AtLeast(3.Times()).Within(TimeSpan.FromSeconds(5))
				.CheckEvery(TimeSpan.FromMilliseconds(10));
			await That(timer).Executed().AtLeast(2.Times()).Within(TimeSpan.FromMilliseconds(100));
		}

		[Fact]
		public async Task Watcher_ShouldSucceed()
		{
			MockFileSystem fileSystem = new();
			fileSystem.InitializeIn("/watched");
			using IFileSystemWatcher watcher = fileSystem.FileSystemWatcher.New("/watched");
			watcher.EnableRaisingEvents = true;
			fileSystem.File.WriteAllText("let-it-be.txt", "let it be");

			await That(watcher).Triggered().Within(TimeSpan.FromSeconds(1));
			await That(watcher).Triggered(c => c.Name == "let-it-be.txt");
			await That(watcher).DidNotTrigger(c => c.Name == "unreleased.txt").Within(TimeSpan.FromMilliseconds(100));

			await That(watcher)
				.Triggered()
				.Matching(c => c.HasName("let-it-be.txt").And.HasChangeType(WatcherChangeTypes.Created))
				.Exactly(1.Times());
		}
	}
}
