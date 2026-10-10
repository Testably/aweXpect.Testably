using System.IO.Abstractions;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed class ArgumentValidation
{
	private static readonly MockFileSystem FileSystem = new();
	private static readonly IDirectoryInfo Directory = FileSystem.DirectoryInfo.New("dir");
	private static readonly IFileInfo File = FileSystem.FileInfo.New("file.txt");
	private static readonly IDriveInfo Drive = FileSystem.DriveInfo.GetDrives()[0];
	private static readonly ChangeDescription Change = ChangeDescriptionTests.Capture(fs => fs.File.WriteAllText("foo.txt", ""));

	[Fact]
	public async Task ChangeDescription_HasPath_WhenExpectedIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(Change).HasPath(null!), "expected");

	[Fact]
	public async Task DirectoryInfo_DoesNotHaveDirectory_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(Directory).DoesNotHaveDirectory(null!), "path");

	[Fact]
	public async Task DirectoryInfo_DoesNotHaveFile_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(Directory).DoesNotHaveFile(null!), "path");

	[Fact]
	public async Task DirectoryInfo_HasDirectory_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(Directory).HasDirectory(null!), "path");

	[Fact]
	public async Task DirectoryInfo_HasFile_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(Directory).HasFile(null!), "path");

	[Fact]
	public async Task DirectoryInfo_HasName_WhenExpectedIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(Directory).HasName(null!), "expected");

	[Fact]
	public async Task DirectoryResult_WithDirectories_WhenExpectationsIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).HasDirectory("dir").WithDirectories(null!),
			"expectations");

	[Fact]
	public async Task DirectoryResult_WithFiles_WhenExpectationsIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).HasDirectory("dir").WithFiles(null!),
			"expectations");

	[Fact]
	public async Task DriveInfo_HasDriveFormat_WhenExpectedIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(Drive).HasDriveFormat(null!), "expected");

	[Fact]
	public async Task DriveInfo_HasName_WhenExpectedIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(Drive).HasName(null!), "expected");

	[Fact]
	public async Task FileInfo_HasContent_WhenExpectedBytesAreNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(File).HasContent((byte[])null!), "expected");

	[Fact]
	public async Task FileInfo_HasContent_EqualTo_WhenExpectedBytesAreNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(File).HasContent().EqualTo((byte[])null!), "expected");

	[Fact]
	public async Task FileInfo_HasContent_NotEqualTo_WhenUnexpectedBytesAreNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(File).HasContent().NotEqualTo((byte[])null!),
			"unexpected");

	[Fact]
	public async Task FileInfo_HasContent_NotSameAs_WhenFilePathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(File).HasContent().NotSameAs(null!), "filePath");

	[Fact]
	public async Task FileInfo_HasContent_SameAs_WhenFilePathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(File).HasContent().SameAs(null!), "filePath");

	[Fact]
	public async Task FileInfo_HasExtension_WhenExpectedIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(File).HasExtension(null!), "expected");

	[Fact]
	public async Task FileInfo_HasName_WhenExpectedIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(File).HasName(null!), "expected");

	[Fact]
	public async Task FileResult_WhoseContent_WhenExpectationsIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).HasFile("file.txt").WhoseContent(null!),
			"expectations");

	[Fact]
	public async Task FileResult_WithContent_EqualTo_WhenExpectedBytesAreNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(
			() => That(FileSystem).HasFile("file.txt").WithContent().EqualTo((byte[])null!), "expected");

	[Fact]
	public async Task FileResult_WithContent_NotEqualTo_WhenUnexpectedBytesAreNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(
			() => That(FileSystem).HasFile("file.txt").WithContent().NotEqualTo((byte[])null!), "unexpected");

	[Fact]
	public async Task FileResult_WithContent_NotSameAs_WhenFilePathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(
			() => That(FileSystem).HasFile("file.txt").WithContent().NotSameAs(null!), "filePath");

	[Fact]
	public async Task FileResult_WithContent_SameAs_WhenFilePathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(
			() => That(FileSystem).HasFile("file.txt").WithContent().SameAs(null!), "filePath");

	[Fact]
	public async Task FileResult_WithContent_WhenExpectedBytesAreNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(
			() => That(FileSystem).HasFile("file.txt").WithContent((byte[])null!), "expected");

	[Fact]
	public async Task FileSystem_DoesNotHaveDirectory_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).DoesNotHaveDirectory(null!), "path");

	[Fact]
	public async Task FileSystem_DoesNotHaveDrive_WhenDriveNameIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).DoesNotHaveDrive(null!), "driveName");

	[Fact]
	public async Task FileSystem_DoesNotHaveFile_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).DoesNotHaveFile(null!), "path");

	[Fact]
	public async Task FileSystem_HasDirectory_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).HasDirectory(null!), "path");

	[Fact]
	public async Task FileSystem_HasDrive_WhenDriveNameIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).HasDrive(null!), "driveName");

	[Fact]
	public async Task FileSystem_HasFile_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem).HasFile(null!), "path");

	[Fact]
	public async Task Recorded_DirectoryInfo_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem.Statistics).Recorded().DirectoryInfo[null!],
			"path");

	[Fact]
	public async Task Recorded_DriveInfo_WhenDriveNameIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem.Statistics).Recorded().DriveInfo[null!],
			"driveName");

	[Fact]
	public async Task Recorded_FileInfo_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem.Statistics).Recorded().FileInfo[null!],
			"path");

	[Fact]
	public async Task Recorded_FileStream_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(() => That(FileSystem.Statistics).Recorded().FileStream[null!],
			"path");

	[Fact]
	public async Task Recorded_FileSystemWatcher_WhenPathIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(
			() => That(FileSystem.Statistics).Recorded().FileSystemWatcher[null!], "path");

	[Fact]
	public async Task Recorded_FileVersionInfo_WhenFileNameIsNull_ShouldThrow()
		=> await ThatThrowsArgumentNullException(
			() => That(FileSystem.Statistics).Recorded().FileVersionInfo[null!], "fileName");

	private static async Task ThatThrowsArgumentNullException(Func<object> act, string paramName)
		=> await That(act).Throws<ArgumentNullException>()
			.WithParamName(paramName).And
			.WithMessage($"The '{paramName}' cannot be null.*").AsWildcard()
			.Because("an invalid argument should fail where the caller passes it, not when the expectation is evaluated");
}
