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
			public async Task OnRootDirectory_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IDirectoryInfo rootDirInfo = fileSystem.DirectoryInfo.New(fileSystem.Path.GetPathRoot(fileSystem.Directory.GetCurrentDirectory())!);

				async Task Act()
				{
					await That(rootDirInfo).WhoseParent.IsNotEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that rootDirInfo
					             whose parent is not empty,
					             but it did throw an InvalidOperationException:
					               Cannot assert on the parent of a root directory because it has no parent.
					             """).And
					.WithInner<InvalidOperationException>(inner => inner
						.HasMessage("Cannot assert on the parent of a root directory because it has no parent."));
			}

			[Fact]
			public async Task HasName_AndWhoseParentHasName_WhenParentNameDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("project").Initialized(p => p
					.WithSubdirectory("src"));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("project/src");

				async Task Act()
				{
					await That(dirInfo).HasName("src").And.WhoseParent.HasName("wrong");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             has name equal to "src" and whose parent has name equal to "wrong",
					             but it was "project", which differs at index 0:
					                ↓ (actual)
					               "project"
					               "wrong"
					                ↑ (expected)
					             """);
			}
		}
	}
}
#endif
