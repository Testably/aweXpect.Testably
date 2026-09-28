using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileVersionInfo
{
	public sealed class HasLegalTrademarks
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetLegalTrademarks("Acme (TM)")
					.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasLegalTrademarks("Acme (TM)").And.HasCompanyName("Acme");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLegalTrademarksDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetLegalTrademarks("Acme (TM)"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasLegalTrademarks("Other (TM)");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             has legal trademarks equal to "Other (TM)",
					             but it was "Acme (TM)", which differs at index 0:
					                ↓ (actual)
					               "Acme (TM)"
					               "Other (TM)"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenLegalTrademarksMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetLegalTrademarks("Acme (TM)"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).HasLegalTrademarks("Acme (TM)");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
