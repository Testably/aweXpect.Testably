using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNameDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasName("bar.txt");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             has name equal to "bar.txt",
					             but it was "foo.txt", which differs at index 0:
					                ↓ (actual)
					               "foo.txt"
					               "bar.txt"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenNameMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasName("foo.txt");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndCaseInsensitiveNameMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(f => f.HasName("FOO.TXT").IgnoringCase());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             does not have name equal to "FOO.TXT" ignoring case,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenNegatedAndNameMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(f => f.HasName("foo.txt"));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             does not have name equal to "foo.txt",
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenNegatedAndWildcardMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText("foo.txt", "");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(f => f.HasName("*.txt").AsWildcard());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             does not have name matching "*.txt",
					             but it did
					             """);
			}
		}
	}
}
