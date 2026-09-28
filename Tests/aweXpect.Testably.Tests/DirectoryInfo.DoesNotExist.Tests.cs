using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class DoesNotExist
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDirectoryDoesNotExist_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotExist();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDirectoryExists_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotExist();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             does not exist,
					             but it did
					             """);
			}
		}
	}
}
