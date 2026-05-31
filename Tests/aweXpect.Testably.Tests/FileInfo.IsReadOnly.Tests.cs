using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class IsReadOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFileIsNotReadOnly_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).IsReadOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             is read-only,
					             but it was not
					             """);
			}

			[Fact]
			public async Task WhenFileDoesNotExist_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).IsReadOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             is read-only,
					             but it did not exist
					             """);
			}

			[Fact]
			public async Task WhenFileIsReadOnly_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetAttributes(path, FileAttributes.ReadOnly);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).IsReadOnly();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
