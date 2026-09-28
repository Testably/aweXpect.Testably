using System.IO.Abstractions;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class HasLength
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFileDoesNotExist_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("missing.txt");

				async Task Act()
				{
					await That(fileInfo).HasLength(0);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             has length 0,
					             but it did not exist
					             """);
			}

			[Fact]
			public async Task WhenLengthDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllBytes("foo.txt", Encoding.UTF8.GetBytes("baz"));
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasLength(5);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileInfo
					             has length 5,
					             but it was 3
					             """);
			}

			[Fact]
			public async Task WhenLengthMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllBytes("foo.txt", Encoding.UTF8.GetBytes("baz"));
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasLength(3);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
