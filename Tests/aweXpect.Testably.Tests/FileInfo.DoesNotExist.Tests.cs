using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class DoesNotExist
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFileDoesNotExist_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotExist();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFileExists_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotExist();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             does not exist,
					             but it did
					             """);
			}
		}
	}
}
