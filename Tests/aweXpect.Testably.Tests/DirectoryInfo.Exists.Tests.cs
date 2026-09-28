using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class Exists
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDirectoryDoesNotExist_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).Exists();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             exists,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenDirectoryExists_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).Exists();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
