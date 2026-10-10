using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class HasDirectory
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubdirectoryExists_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo/bar");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasDirectory("bar");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubdirectoryMissing_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasDirectory("bar");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             has directory "bar",
					             but it did not exist
					             """);
			}
		}
	}

	// TODO: Re-enable once https://github.com/aweXpect/aweXpect issue/PR for
	// ExpectationBuilder.ForWhich chaining is merged. Multi-level .Which chains
	// fail today because the outer WhichNode receives the original value rather
	// than the matchingValue projected through the inner WhichNode.
	//
	// public sealed class HasDirectoryDeeplyNested
	// {
	//     public sealed class Tests
	//     {
	//         [Fact]
	//         public async Task DeeplyNested_ShouldSucceed()
	//         {
	//             MockFileSystem fileSystem = new();
	//             fileSystem.Initialize().WithSubdirectory("project").Initialized(p => p
	//                 .WithSubdirectory("src").Initialized(s => s
	//                     .WithFile("Program.cs").Which(f => f.HasStringContent("xxxxxxxxxxxxxxxx"))));
	//             IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("project");
	//
	//             async Task Act()
	//                 => await That(dirInfo).HasDirectory("src").Which
	//                     .HasFile("Program.cs").Which.HasLength(16);
	//
	//             await That(Act).DoesNotThrow();
	//         }
	//     }
	// }
}
