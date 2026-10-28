#if NET10_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileInfo
{
	public sealed class WhoseParent
	{
		public sealed class Tests
		{
			[Test]
			public async Task HasName_WhenParentNameMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("logs").Initialized(d => d
					.WithFile("today.log"));
				IFileInfo fileInfo = fileSystem.FileInfo.New("logs/today.log");

				async Task Act()
				{
					await That(fileInfo).WhoseParent.HasName("logs");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IsNotEmpty_WhenFileDoesNotExist_ShouldStillEvaluateParent()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("logs").Initialized(d => d
					.WithFile("other.log"));
				IFileInfo fileInfo = fileSystem.FileInfo.New("logs/missing.log");

				async Task Act()
				{
					await That(fileInfo).WhoseParent.IsNotEmpty();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IsNotEmpty_WhenFileIsInNonEmptyDirectory_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("logs").Initialized(d => d
					.WithFile("today.log"));
				IFileInfo fileInfo = fileSystem.FileInfo.New("logs/today.log");

				async Task Act()
				{
					await That(fileInfo).WhoseParent.IsNotEmpty();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HasName_AndWhoseParentHasName_WhenParentNameDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("logs").Initialized(d => d
					.WithFile("today.log"));
				IFileInfo fileInfo = fileSystem.FileInfo.New("logs/today.log");

				async Task Act()
				{
					await That(fileInfo).HasName("today.log").And.WhoseParent.HasName("wrong");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             has name equal to "today.log" and whose parent has name equal to "wrong",
					             but the parent was "logs", which differs at index 0:
					                ↓ (actual)
					               "logs"
					               "wrong"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task OnRootFile_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New(
					fileSystem.Path.GetPathRoot(fileSystem.Directory.GetCurrentDirectory())!);

				async Task Act()
				{
					await That(fileInfo).WhoseParent.IsNotEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that fileInfo
					             whose parent is not empty,
					             but the parent did throw an InvalidOperationException:
					               Cannot assert on the parent directory of the file because it has none.
					             """).And
					.WithInner<InvalidOperationException>(inner => inner
						.HasMessage("Cannot assert on the parent directory of the file because it has none."));
			}
		}
	}
}

public sealed partial class ThatFileInfo
{
	public sealed class WhichThenWhoseParent
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenChainedAfterFileWhich_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.Initialize().WithSubdirectory("logs").Initialized(d => d
					.WithFile("today.log").Which(f => f.HasStringContent(new string('x', 2048))));

				async Task Act()
					=> await That(sut).HasFile("logs/today.log")
						.Which.HasLength(2048).And.WhoseParent.IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenChainedAfterNestedWhich_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.Initialize().WithSubdirectory("project").Initialized(p => p
					.WithSubdirectory("src").Initialized(s => s
						.WithFile("Program.cs").Which(f => f.HasStringContent("xxxxxxxxxxxxxxxx"))));

				async Task Act()
					=> await That(sut).HasDirectory("project")
						.Which.HasDirectory("src")
						.Which.HasFile("Program.cs")
						.Which.HasLength(16).And.WhoseParent.HasName("src");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenChainedAfterDirectoryWhich_ShouldSucceed()
			{
				MockFileSystem sut = new();
				sut.Initialize().WithSubdirectory("project").Initialized(p => p
					.WithSubdirectory("src").Initialized(s => s.WithFile("Program.cs")));

				async Task Act()
					=> await That(sut).HasDirectory("project")
						.Which.HasDirectory("src")
						.Which.IsNotEmpty().And.WhoseParent.HasName("project");

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
