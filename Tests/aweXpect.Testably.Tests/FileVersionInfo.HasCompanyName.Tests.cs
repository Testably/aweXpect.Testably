using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileVersionInfo
{
	public sealed class HasCompanyName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetCompanyName("Acme")
					.SetProductName("Anvil"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasCompanyName("Acme").And.HasProductName("Anvil");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCompanyNameDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasCompanyName("Contoso");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             has company name equal to "Contoso",
					             but it was "Acme", which differs at index 0:
					                ↓ (actual)
					               "Acme"
					               "Contoso"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenCompanyNameMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasCompanyName("Acme");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
