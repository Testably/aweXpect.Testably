using System.IO.Abstractions;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed partial class HasContent
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenContentDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "bar2");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent("bar");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileInfo
					             has content equal to "bar",
					             but it was "bar2" with a length of 4 which is longer than the expected length of 3 and has superfluous:
					               "2"

					             File content:
					             bar2
					             """);
			}

			[Fact]
			public async Task WhenContentMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "bar");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent("bar");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFileDoesNotExist_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent("bar");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileInfo
					             has content equal to "bar",
					             but it did not exist
					             """);
			}
		}


		public sealed class BinaryTests
		{
			[Fact]
			public async Task WhenContentIsDifferent_ShouldFail()
			{
				byte[] content = Encoding.UTF8.GetBytes("baz");
				byte[] expected = Encoding.UTF8.GetBytes("bar");
				string path = "foo.txt";
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllBytes(path, content);
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent(expected);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             has content equal to expected,
					             but it differed
					             """);
			}

			[Fact]
			public async Task WhenContentMatches_ShouldSucceed()
			{
				byte[] content = Encoding.UTF8.GetBytes("baz");
				string path = "foo.txt";
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllBytes(path, content);
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent(content);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFileDoesNotExist_ShouldFail()
			{
				byte[] expected = Encoding.UTF8.GetBytes("bar");
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent(expected);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             has content equal to expected,
					             but it did not exist
					             """);
			}
		}

		public sealed class AsWildcardTests
		{
			[Fact]
			public async Task WhenContentIsDifferent_ShouldFail()
			{
				string path = "foo.txt";
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText(path, "baz");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent("b?").AsWildcard();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             has content matching "b?" as wildcard,
					             but it did not match:
					               ↓ (actual)
					               "baz"
					               "b?"
					               ↑ (wildcard pattern)

					             File content:
					             baz
					             """);
			}

			[Fact]
			public async Task WhenContentMatches_ShouldSucceed()
			{
				string path = "foo.txt";
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllText(path, "bar");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent("ba?").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
