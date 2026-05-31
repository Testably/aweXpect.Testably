#if NET10_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class WhoseParent
	{
		public sealed class Tests
		{
			[Fact]
			public async Task HasName_WhenParentNameMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("project").Initialized(p => p
					.WithSubdirectory("src"));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("project/src");

				async Task Act()
				{
					await That(dirInfo).WhoseParent.HasName("project");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IsNotEmpty_WhenParentIsNonEmpty_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("project").Initialized(p => p
					.WithSubdirectory("src"));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("project/src");

				async Task Act()
				{
					await That(dirInfo).WhoseParent.IsNotEmpty();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task OnRootDirectory_ShouldThrow()
			{
				MockFileSystem fileSystem = new();
				IDirectoryInfo rootDirInfo = fileSystem.DirectoryInfo.New(fileSystem.Path.GetPathRoot(fileSystem.Directory.GetCurrentDirectory())!);

				async Task Act()
				{
					await That(rootDirInfo).WhoseParent.IsNotEmpty();
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Cannot assert on the parent of a root directory because it has no parent.");
			}
		}
	}
}
#endif
