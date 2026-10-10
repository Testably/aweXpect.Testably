using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class DoesNotHaveFile
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFileDoesNotExist_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotHaveFile("bar.txt");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFileExists_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("foo").Initialized(d => d
					.WithFile("bar.txt"));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotHaveFile("bar.txt");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             does not have file "bar.txt",
					             but it did
					             """);
			}
		}
	}
}
