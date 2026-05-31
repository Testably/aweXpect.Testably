using System.IO.Abstractions;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed partial class HasContent
	{
		public sealed class BinaryNegated
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldSucceed()
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
						await That(fileInfo).DoesNotComplyWith(it => it.HasContent(expected));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldFail()
				{
					byte[] content = Encoding.UTF8.GetBytes("baz");
					string path = "foo.txt";
					MockFileSystem fileSystem = new();
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllBytes(path, content);
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).DoesNotComplyWith(it => it.HasContent(content));
					}

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that fileInfo
						             has content different from content,
						             but it did match
						             """);
				}
			}
		}
	}
}
