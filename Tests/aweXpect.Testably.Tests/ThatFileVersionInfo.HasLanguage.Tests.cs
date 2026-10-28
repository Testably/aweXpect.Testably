using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileVersionInfo
{
	public sealed class HasLanguage
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetLanguage("English (United States)")
					.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasLanguage("English (United States)").And.HasCompanyName("Acme");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLanguageDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetLanguage("English (United States)"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasLanguage("German (Germany)");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             has language equal to "German (Germany)",
					             but it was "English (United States)", which differs at index 0:
					                ↓ (actual)
					               "English (United States)"
					               "German (Germany)"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenLanguageMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetLanguage("English (United States)"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasLanguage("English (United States)");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
