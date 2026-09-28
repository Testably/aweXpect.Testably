using System.IO.Abstractions;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed partial class HasContent
	{
		public class EqualTo
		{
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
						await That(fileInfo).HasContent().EqualTo(expected);
					}

					await That(Act).Throws()
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
						await That(fileInfo).HasContent().EqualTo(content);
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class StringTests
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
						await That(fileInfo).HasContent().EqualTo("bar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileInfo
						             has content equal to "bar",
						             but it was "baz", which differs at index 2:
						                  ↓ (actual)
						               "baz"
						               "bar"
						                  ↑ (expected)

						             File content:
						             baz
						             """);
				}

				[Fact]
				public async Task WhenContentMatches_ShouldSucceed()
				{
					string path = "foo.txt";
					string content = "bar";
					MockFileSystem fileSystem = new();
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, content);
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().EqualTo(content);
					}

					await That(Act).DoesNotThrow();
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
						await That(fileInfo).HasContent().EqualTo("b?").AsWildcard();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileInfo
						             has content matching "b?",
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
						await That(fileInfo).HasContent().EqualTo("ba?").AsWildcard();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
