using System.IO.Abstractions;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class HasLengthNegated
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenLengthDiffers_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllBytes("foo.txt", Encoding.UTF8.GetBytes("baz"));
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(it => it.HasLength(5));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				// ReSharper disable once MethodHasAsyncOverload
				fileSystem.File.WriteAllBytes("foo.txt", Encoding.UTF8.GetBytes("baz"));
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(it => it.HasLength(3));
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileInfo
					             does not have length 3,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenFileDoesNotExist_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				IFileInfo fileInfo = fileSystem.FileInfo.New("missing.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(it => it.HasLength(0));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
