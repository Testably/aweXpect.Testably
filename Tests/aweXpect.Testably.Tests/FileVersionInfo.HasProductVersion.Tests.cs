using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileVersionInfo
{
	public sealed class HasProductVersion
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetProductVersion("1.2")
					.SetFileVersion("1.2.3.4"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasProductVersion("1.2").And.HasFileVersion("1.2.3.4");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenProductVersionDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetProductVersion("1.2"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasProductVersion("9.9");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             has product version equal to "9.9",
					             but it was "1.2", which differs at index 0:
					                ↓ (actual)
					               "1.2"
					               "9.9"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenProductVersionMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetProductVersion("1.2"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasProductVersion("1.2");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
