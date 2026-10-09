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
			public async Task EqualTo_WhenChainedWithAnd_ShouldKeepTheAnd()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "bar");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).Exists().And.HasContent().EqualTo("baz");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileInfo
					             exists and has content equal to "baz",
					             but it was "bar", which differs at index 2:
					                  ↓ (actual)
					               "bar"
					               "baz"
					                  ↑ (expected)

					             File content:
					             bar
					             """);
			}

			[Fact]
			public async Task EqualTo_WhenUsedForAllItems_ShouldNotStartWithASpace()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "bar");
				IFileInfo[] fileInfos = [fileSystem.FileInfo.New("foo.txt"),];

				async Task Act()
				{
					await That(fileInfos).All().ComplyWith(f => f.HasContent().EqualTo("baz"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileInfos
					             has content equal to "baz" for all items,
					             but none of 1 did

					             Not matching items:
					             [
					               foo.txt
					             ]

					             Collection:
					             [
					               foo.txt
					             ]

					             File content (item [0]):
					             bar
					             """).IgnoringNewlineStyle();
			}

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
					             but it was "bar2" with a length of 4, which is longer than the expected length of 3 and has superfluous:
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

			[Fact]
			public async Task WhenEvaluatedForMultipleItems_ShouldNotShowContentOfPreviousItem()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("a.txt", "abc");
				IFileInfo[] fileInfos = [fileSystem.FileInfo.New("a.txt"), fileSystem.FileInfo.New("missing.txt"),];

				async Task Act()
				{
					await That(fileInfos).All().ComplyWith(f => f.HasContent("abc"));
				}

				XunitException exception = await That(Act).Throws<XunitException>();
				await That(exception.Message).DoesNotContain("File content")
					.Because("the missing file has no content, so the content of the previous item must not leak into it");
			}

			[Fact]
			public async Task WhenIgnoringCase_ShouldMentionTheOptionOnce()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "bar");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).HasContent("BAZ").IgnoringCase();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileInfo
					             has content equal to "BAZ" ignoring case,
					             but it was "bar", which differs at index 2:
					                  ↓ (actual)
					               "bar"
					               "BAZ"
					                  ↑ (expected)

					             File content:
					             bar
					             """);
			}

			[Fact]
			public async Task WhenNegated_WhenContentMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "bar");
				IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(it => it.HasContent("bar"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileInfo
					             does not have content equal to "bar",
					             but it was "bar"

					             File content:
					             bar
					             """)
					.Because("the negation belongs to the verb only");
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

				await That(Act).Throws()
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
					await That(fileInfo).HasContent("ba?").AsWildcard();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
