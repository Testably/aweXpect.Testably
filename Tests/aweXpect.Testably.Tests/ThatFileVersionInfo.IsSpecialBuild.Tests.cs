using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileVersionInfo
{
	public sealed class IsSpecialBuild
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldSupportAndComposition()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v
					.SetIsSpecialBuild(true)
					.SetCompanyName("Acme"));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).IsSpecialBuild().And.HasCompanyName("Acme");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInfoIsNotSpecialBuild_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetIsSpecialBuild(false));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).IsSpecialBuild();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             is special build,
					             but it was not
					             """);
			}

			[Test]
			public async Task WhenInfoIsSpecialBuild_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetIsSpecialBuild(true));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).IsSpecialBuild();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegated_AndInfoIsSpecialBuild_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.WithFileVersionInfo("*.dll", v => v.SetIsSpecialBuild(true));
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("Acme.dll", "");
				IFileVersionInfo info = fileSystem.FileVersionInfo.GetVersionInfo("Acme.dll");

				async Task Act()
				{
					await That(info).DoesNotComplyWith(it => it.IsSpecialBuild());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that info
					             is not special build,
					             but it was
					             """);
			}
		}
	}
}
