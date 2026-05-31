using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasFileSystemType
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotHaveFileSystemType_WhenItDiffers_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveFileSystemType(FileSystemTypes.Directory);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHaveFileSystemType_WhenItMatches_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveFileSystemType(FileSystemTypes.File);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that change
					             does not have file system type File,
					             but it did
					             """);
			}

			[Fact]
			public async Task DoesNotHaveFileSystemType_WhenUnexpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveFileSystemType(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected");
				await That(Act).Throws<ArgumentException>()
					.WithMessage("The unexpected file system type must include at least one flag.*").AsWildcard();
			}

			[Fact]
			public async Task WhenExpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasFileSystemType(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected");
				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected file system type must include at least one flag.*").AsWildcard();
			}

			[Fact]
			public async Task WhenFileSystemTypeDiffers_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasFileSystemType(FileSystemTypes.Directory);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that change
					             has file system type Directory,
					             but it was File
					             """);
			}

			[Fact]
			public async Task WhenFileSystemTypeMatches_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasFileSystemType(FileSystemTypes.File);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ChangeDescription? change = null;

				async Task Act()
				{
					await That(change!).HasFileSystemType(FileSystemTypes.File);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that change
					             has file system type File,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedPartiallyOverlapsActual_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change)
						.HasFileSystemType(FileSystemTypes.File | FileSystemTypes.Directory);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that change
					             has file system type DirectoryOrFile,
					             but it was File
					             """);
			}
		}
	}
}
