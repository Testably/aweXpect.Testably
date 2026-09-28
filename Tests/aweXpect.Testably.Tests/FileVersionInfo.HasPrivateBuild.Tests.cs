using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileVersionInfo
{
	public sealed class HasPrivateBuild
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetPrivateBuild("Acme")
					.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasPrivateBuild("Acme").And.HasCompanyName("Acme");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPrivateBuildDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetPrivateBuild("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasPrivateBuild("Contoso");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             has private build equal to "Contoso",
					             but it was "Acme", which differs at index 0:
					                ↓ (actual)
					               "Acme"
					               "Contoso"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenPrivateBuildMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetPrivateBuild("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasPrivateBuild("Acme");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
