using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileVersionInfo
{
	public sealed class HasProductBuildPart
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetProductVersion("5.6.7.8")
					.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasProductBuildPart(7).And.HasCompanyName("Acme");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenProductBuildPartDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetProductVersion("5.6.7.8"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasProductBuildPart(9);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             has product build part 9,
					             but it was 7
					             """);
			}

			[Fact]
			public async Task WhenProductBuildPartMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetProductVersion("5.6.7.8"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasProductBuildPart(7);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
