using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class HasExtension
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExtensionDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasExtension(".md");
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             has extension equal to ".md",
					             but it was ".txt" which differs at index 1:
					                 ↓ (actual)
					               ".txt"
					               ".md"
					                 ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenExtensionMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasExtension(".txt");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndExtensionMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(f => f.HasExtension(".txt"));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             does not have extension not equal to ".txt",
					             but it did
					             """);
			}
		}
	}
}
