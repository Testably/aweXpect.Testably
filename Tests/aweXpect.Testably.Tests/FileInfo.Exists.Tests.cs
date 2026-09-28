using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class Exists
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFileDoesNotExist_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).Exists();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             exists,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenFileExists_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).Exists();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
