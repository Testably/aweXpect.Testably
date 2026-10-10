using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class DoesNotHaveDirectory
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubdirectoryExists_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo/bar");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotHaveDirectory("bar");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             does not have directory "bar",
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenSubdirectoryMissing_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotHaveDirectory("bar");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
