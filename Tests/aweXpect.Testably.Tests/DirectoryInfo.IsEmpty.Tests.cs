using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class IsEmpty
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDirectoryHasFiles_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("foo").Initialized(d => d
					.WithFile("bar.txt"));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).IsEmpty();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that dirInfo
					             is empty,
					             but it was not
					             """);
			}

			[Fact]
			public async Task WhenDirectoryDoesNotExist_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("missing");

				async Task Act()
				{
					await That(dirInfo).IsEmpty();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that dirInfo
					             is empty,
					             but it did not exist
					             """);
			}

			[Fact]
			public async Task WhenDirectoryIsEmpty_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).IsEmpty();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
