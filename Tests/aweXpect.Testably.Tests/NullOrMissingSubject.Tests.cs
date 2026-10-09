using System.IO;
using System.IO.Abstractions;
using System.Text;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;
using Testably.Abstractions.Testing.TimeSystem;

namespace aweXpect.Testably.Tests;

public sealed class NullOrMissingSubject
{
	public sealed class NullSubjectTests
	{
		[Fact]
		public async Task ChangeDescription_DoesNotHaveChangeType_ShouldFail()
		{
			ChangeDescription subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(c => c.HasChangeType(WatcherChangeTypes.Created));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task ChangeDescription_DoesNotHaveFileSystemType_ShouldFail()
		{
			ChangeDescription subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(c => c.HasFileSystemType(FileSystemTypes.File));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task ChangeDescription_DoesNotHaveName_ShouldFail()
		{
			ChangeDescription subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(c => c.HasName("foo.txt"));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task ChangeDescription_DoesNotHaveNotifyFilters_ShouldFail()
		{
			ChangeDescription subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(c => c.HasNotifyFilters(NotifyFilters.FileName));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task DriveInfo_DoesNotHaveAvailableFreeSpace_ShouldFail()
		{
			IDriveInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(d => d.HasAvailableFreeSpace(1));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task DriveInfo_DoesNotHaveDriveFormat_ShouldFail()
		{
			IDriveInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(d => d.HasDriveFormat("NTFS"));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task DriveInfo_DoesNotHaveDriveType_ShouldFail()
		{
			IDriveInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(d => d.HasDriveType(DriveType.Fixed));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task DriveInfo_DoesNotHaveName_ShouldFail()
		{
			IDriveInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(d => d.HasName("C:\\"));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task DriveInfo_DoesNotHaveTotalFreeSpace_ShouldFail()
		{
			IDriveInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(d => d.HasTotalFreeSpace(1));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task DriveInfo_DoesNotHaveTotalSize_ShouldFail()
		{
			IDriveInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(d => d.HasTotalSize(1));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task DriveInfo_DoesNotHaveVolumeLabel_ShouldFail()
		{
			IDriveInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(d => d.HasVolumeLabel("label"));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task DriveInfo_IsNotReady_ShouldFail()
		{
			IDriveInfo subject = null!;

			async Task Act()
				=> await That(subject).IsNotReady();

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task FileSystem_DidNotTriggerNotification_ShouldFail()
		{
			MockFileSystem subject = null!;

			async Task Act()
				=> await That(subject).DidNotTriggerNotification().Within(TimeSpan.FromMilliseconds(10));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task FileSystemWatcher_DidNotTrigger_ShouldFail()
		{
			IFileSystemWatcher subject = null!;

			async Task Act()
				=> await That(subject).DidNotTrigger().Within(TimeSpan.FromMilliseconds(10));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task FileVersionInfo_DoesNotHaveComments_ShouldFail()
		{
			IFileVersionInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(v => v.HasComments("comments"));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task FileVersionInfo_DoesNotHaveFileMajorPart_ShouldFail()
		{
			IFileVersionInfo subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(v => v.HasFileMajorPart(1));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task FileVersionInfo_IsNotDebug_ShouldFail()
		{
			IFileVersionInfo subject = null!;

			async Task Act()
				=> await That(subject).IsNotDebug();

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}

		[Fact]
		public async Task Timer_DidNotExecute_ShouldFail()
		{
			ITimerMock subject = null!;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(t => t.Executed().Within(TimeSpan.FromMilliseconds(10)));

			await That(Act).Throws<XunitException>().WithMessage("*but it was <null>").AsWildcard()
				.Because("a null subject fails an expectation that inspects it and its negation alike");
		}
	}

	public sealed class MissingSubjectTests
	{
		[Fact]
		public async Task DirectoryInfo_DoesNotHaveCreationTime_ShouldFail()
		{
			MockFileSystem fileSystem = new();
			IDirectoryInfo subject = fileSystem.DirectoryInfo.New("missing");

			async Task Act()
				=> await That(subject).DoesNotComplyWith(d => d.HasCreationTime(DateTime.Now));

			await That(Act).Throws<XunitException>().WithMessage("*but it did not exist").AsWildcard()
				.Because("a missing directory cannot be inspected, so the negation fails as well");
		}

		[Fact]
		public async Task FileInfo_DoesNotHaveBinaryContent_ShouldFail()
		{
			MockFileSystem fileSystem = new();
			IFileInfo subject = fileSystem.FileInfo.New("missing.txt");

			async Task Act()
				=> await That(subject).DoesNotComplyWith(f => f.HasContent(Encoding.UTF8.GetBytes("foo")));

			await That(Act).Throws<XunitException>().WithMessage("*but it did not exist").AsWildcard()
				.Because("a missing file cannot be inspected, so the negation fails as well");
		}

		[Fact]
		public async Task FileInfo_DoesNotHaveContent_ShouldFail()
		{
			MockFileSystem fileSystem = new();
			IFileInfo subject = fileSystem.FileInfo.New("missing.txt");

			async Task Act()
				=> await That(subject).DoesNotComplyWith(f => f.HasContent("foo"));

			await That(Act).Throws<XunitException>().WithMessage("*but it did not exist").AsWildcard()
				.Because("a missing file cannot be inspected, so the negation fails as well");
		}

		[Fact]
		public async Task FileInfo_HasContentNotSameAsMissingFile_ShouldFail()
		{
			MockFileSystem fileSystem = new();
			fileSystem.File.WriteAllText("foo.txt", "foo");
			IFileInfo subject = fileSystem.FileInfo.New("foo.txt");

			async Task Act()
				=> await That(subject).HasContent().NotSameAs("missing.txt");

			await That(Act).Throws<XunitException>().WithMessage("*but it did not contain any file at*").AsWildcard()
				.Because("a missing file cannot be compared, so the negation fails as well");
		}

		[Fact]
		public async Task FileInfo_IsNotReadOnly_ShouldFail()
		{
			MockFileSystem fileSystem = new();
			IFileInfo subject = fileSystem.FileInfo.New("missing.txt");

			async Task Act()
				=> await That(subject).IsNotReadOnly();

			await That(Act).Throws<XunitException>().WithMessage("*but it did not exist").AsWildcard()
				.Because("a missing file cannot be inspected, so the negation fails as well");
		}

		[Fact]
		public async Task FileSystem_HasFileWithContentNotSameAsMissingFile_ShouldFail()
		{
			MockFileSystem subject = new();
			subject.File.WriteAllText("foo.txt", "foo");

			async Task Act()
				=> await That(subject).HasFile("foo.txt").WithContent().NotSameAs("missing.txt");

			await That(Act).Throws<XunitException>().WithMessage("*but it did not contain any file at*").AsWildcard()
				.Because("a missing file cannot be compared, so the negation fails as well");
		}
	}
}
