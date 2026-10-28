using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileInfo
{
	public sealed class DoesNotExist
	{
		public sealed class Tests
		{
			[Test]
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

			[Test]
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
