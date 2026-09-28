using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileVersionInfo
{
	public sealed class HasSpecialBuild
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetSpecialBuild("Acme")
					.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasSpecialBuild("Acme").And.HasCompanyName("Acme");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSpecialBuildDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetSpecialBuild("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasSpecialBuild("Contoso");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             has special build equal to "Contoso",
					             but it was "Acme", which differs at index 0:
					                ↓ (actual)
					               "Acme"
					               "Contoso"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenSpecialBuildMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetSpecialBuild("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasSpecialBuild("Acme");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
