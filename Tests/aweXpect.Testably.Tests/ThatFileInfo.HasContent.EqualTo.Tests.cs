using System.IO.Abstractions;
using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileInfo
{
	public sealed partial class HasContent
	{
		public class EqualTo
		{
			public sealed class BinaryTests
			{
				[Test]
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

				[Test]
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

				[Test]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					byte[] expected = Encoding.UTF8.GetBytes("bar");
					MockFileSystem fileSystem = new();
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().EqualTo(expected);
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that fileInfo
						             has content equal to expected,
						             but it did not exist
						             """);
				}

				[Test]
				public async Task WhenNegated_WhenFileDoesNotExist_ShouldFail()
				{
					byte[] expected = Encoding.UTF8.GetBytes("bar");
					MockFileSystem fileSystem = new();
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).DoesNotComplyWith(it => it.HasContent().EqualTo(expected));
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that fileInfo
						             does not have content equal to expected,
						             but it did not exist
						             """)
						.Because("a missing file has no content to compare, so the negation fails as well");
				}
			}

			public sealed class StringTests
			{
				[Test]
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

				[Test]
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

				[Test]
				public async Task WhenEvaluatedForMultipleItems_ShouldNotShowContentOfPreviousItem()
				{
					MockFileSystem fileSystem = new();
					fileSystem.File.WriteAllText("a.txt", "abc");
					IFileInfo[] fileInfos = [fileSystem.FileInfo.New("a.txt"), fileSystem.FileInfo.New("missing.txt"),];

					async Task Act()
					{
						await That(fileInfos).All().ComplyWith(f => f.HasContent().EqualTo("abc"));
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that fileInfos
						             has content equal to "abc" for all items,
						             but for the item at index 1, it did not exist

						             Collection:
						             [
						               a.txt,
						               missing.txt
						             ]
						             """)
						.Because("the missing file has no content, so the content of the previous item must not leak into it");
				}

				[Test]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().EqualTo("bar");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that fileInfo
						             has content equal to "bar",
						             but it did not exist
						             """);
				}

				[Test]
				public async Task WhenNegated_WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).DoesNotComplyWith(it => it.HasContent().EqualTo("bar"));
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that fileInfo
						             does not have content equal to "bar",
						             but it did not exist
						             """)
						.Because("a missing file has no content to compare, so the negation fails as well");
				}
			}

			public sealed class AsWildcardTests
			{
				[Test]
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

				[Test]
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
