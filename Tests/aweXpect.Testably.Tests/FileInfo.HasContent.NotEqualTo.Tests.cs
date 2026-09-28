using System.IO.Abstractions;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed partial class HasContent
	{
		public class NotEqualTo
		{
			public sealed class BinaryTests
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
						await That(fileInfo).HasContent().NotEqualTo(expected);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldFail()
				{
					string path = "foo.txt";
					byte[] content = Encoding.UTF8.GetBytes("baz");
					MockFileSystem fileSystem = new();
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllBytes(path, content);
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotEqualTo(content);
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileInfo
						             has content different from content,
						             but it did match
						             """);
				}
			}

			public sealed class StringTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldSucceed()
				{
					string path = "foo.txt";
					MockFileSystem fileSystem = new();
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "baz");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotEqualTo("bar");
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldFail()
				{
					string path = "foo.txt";
					string content = "bar";
					MockFileSystem fileSystem = new();
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, content);
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotEqualTo(content);
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileInfo
						             has content not equal to "bar",
						             but it did match

						             File content:
						             bar
						             """);
				}
			}

			public sealed class WildcardTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldSucceed()
				{
					string path = "foo.txt";
					MockFileSystem fileSystem = new();
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "baz");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotEqualTo("b?").AsWildcard();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenContentMatches_ShouldFail()
				{
					string path = "foo.txt";
					MockFileSystem fileSystem = new();
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "bar");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotEqualTo("ba?").AsWildcard();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileInfo
						             has content not matching "ba?",
						             but it did match

						             File content:
						             bar
						             """);
				}
			}
		}
	}
}
