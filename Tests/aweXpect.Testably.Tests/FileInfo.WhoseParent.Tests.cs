#if NET10_0_OR_GREATER
using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class WhoseParent
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
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

			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             has name equal to "today.log" whose parent has name equal to "wrong",
					             but it was "logs" which differs at index 0:
					                ↓ (actual)
					               "logs"
					               "wrong"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task OnRootFile_ShouldThrow()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New(
					fileSystem.Path.GetPathRoot(fileSystem.Directory.GetCurrentDirectory())!);

				async Task Act()
				{
					await That(fileInfo).WhoseParent.IsNotEmpty();
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Cannot assert on the parent directory of the file because it has none.");
			}
		}
	}
}

// TODO: Re-enable once https://github.com/aweXpect/aweXpect issue/PR for
// ExpectationBuilder.ForWhich chaining is merged. The fix makes ForWhich
// drain any pending _whichNode before creating a new one, which is required
// for .Which → WhoseParent to compose.
//
// public sealed partial class FileInfo
// {
//     public sealed class WhichThenWhoseParent
//     {
//         public sealed class Tests
//         {
//             [Fact]
//             public async Task HasFile_Which_HasLength_And_WhoseParent_IsNotEmpty_ShouldSucceed()
//             {
//                 MockFileSystem sut = new();
//                 sut.Initialize().WithSubdirectory("logs").Initialized(d => d
//                     .WithFile("today.log").Which(f => f.HasStringContent(new string('x', 2048))));
//
//                 async Task Act()
//                     => await That(sut).HasFile("logs/today.log")
//                         .Which.HasLength(2048).And.WhoseParent.IsNotEmpty();
//
//                 await That(Act).DoesNotThrow();
//             }
//
//             [Fact]
//             public async Task HasDirectory_Which_HasFile_Which_HasLength_And_WhoseParent_HasName_ShouldSucceed()
//             {
//                 MockFileSystem sut = new();
//                 sut.Initialize().WithSubdirectory("project").Initialized(p => p
//                     .WithSubdirectory("src").Initialized(s => s
//                         .WithFile("Program.cs").Which(f => f.HasStringContent("xxxxxxxxxxxxxxxx"))));
//
//                 async Task Act()
//                     => await That(sut).HasDirectory("project")
//                         .Which.HasDirectory("src")
//                         .Which.HasFile("Program.cs")
//                         .Which.HasLength(16).And.WhoseParent.HasName("src");
//
//                 await That(Act).DoesNotThrow();
//             }
//
//             [Fact]
//             public async Task IsNotEmpty_And_WhoseParent_HasName_AfterDirectoryDescent_ShouldSucceed()
//             {
//                 MockFileSystem sut = new();
//                 sut.Initialize().WithSubdirectory("project").Initialized(p => p
//                     .WithSubdirectory("src").Initialized(s => s.WithFile("Program.cs")));
//
//                 async Task Act()
//                     => await That(sut).HasDirectory("project")
//                         .Which.HasDirectory("src")
//                         .Which.IsNotEmpty().And.WhoseParent.HasName("project");
//
//                 await That(Act).DoesNotThrow();
//             }
//         }
//     }
// }
#endif
