using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatDirectoryInfo
{
	public sealed class HasDirectory
	{
		public sealed class Tests
		{
			[Test]
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

			[Test]
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

	public sealed class HasDirectoryDeeplyNested
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenWhichIsChained_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Initialize().WithSubdirectory("project").Initialized(p => p
					.WithSubdirectory("src").Initialized(s => s
						.WithFile("Program.cs").Which(f => f.HasStringContent("xxxxxxxxxxxxxxxx"))));
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("project");

				async Task Act()
					=> await That(dirInfo).HasDirectory("src").Which
						.HasFile("Program.cs").Which.HasLength(16);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
