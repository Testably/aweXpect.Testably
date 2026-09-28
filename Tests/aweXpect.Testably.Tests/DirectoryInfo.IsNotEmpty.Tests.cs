using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class IsNotEmpty
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenDirectoryHasFiles_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("foo").Initialized(d => d
					.WithFile("bar.txt"));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).IsNotEmpty();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDirectoryIsEmpty_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).IsNotEmpty();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             is not empty,
					             but it was
					             """);
			}
		}
	}
}
