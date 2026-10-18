using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileVersionInfo
{
	public sealed class HasOriginalFilename
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetOriginalFilename("Acme.dll")
					.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasOriginalFilename("Acme.dll").And.HasCompanyName("Acme");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOriginalFilenameDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetOriginalFilename("Acme.dll"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasOriginalFilename("Other.dll");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             has original filename equal to "Other.dll",
					             but it was "Acme.dll", which differs at index 0:
					                ↓ (actual)
					               "Acme.dll"
					               "Other.dll"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenOriginalFilenameMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetOriginalFilename("Acme.dll"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasOriginalFilename("Acme.dll");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
