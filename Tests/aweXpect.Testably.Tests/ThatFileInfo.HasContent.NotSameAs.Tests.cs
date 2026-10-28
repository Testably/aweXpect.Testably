using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileInfo
{
	public sealed partial class HasContent
	{
		public class NotSameAs
		{
			public sealed class StringTests
			{
				[Test]
				public async Task WhenContentIsDifferent_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "baz");
					fileSystem.File.WriteAllText(expectedPath, "bar");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotSameAs(expectedPath);
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenContentMatches_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string content = "bar";
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, content);
					fileSystem.File.WriteAllText(expectedPath, content);
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotSameAs(expectedPath);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that fileInfo
						              does not have the same content as file {Formatter.Format(fullExpectedPath)},
						              but it did match

						              File content:
						              bar
						              """);
				}

				[Test]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					fileSystem.File.WriteAllText(expectedPath, "bar");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotSameAs(expectedPath);
					}

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that fileInfo
						              does not have the same content as file {Formatter.Format(fullExpectedPath)},
						              but it did not exist
						              """)
						.Because("a missing file has no content that could differ");
				}

				[Test]
				public async Task WhenNegated_WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					fileSystem.File.WriteAllText(expectedPath, "bar");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).DoesNotComplyWith(it => it.HasContent().NotSameAs(expectedPath));
					}

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that fileInfo
						              has the same content as file {Formatter.Format(fullExpectedPath)},
						              but it did not exist
						              """);
				}
			}

			public sealed class AsWildcardTests
			{
				[Test]
				public async Task WhenContentIsDifferent_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "baz");
					fileSystem.File.WriteAllText(expectedPath, "b?");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotSameAs(expectedPath).AsWildcard();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenContentMatches_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "bar");
					fileSystem.File.WriteAllText(expectedPath, "ba?");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().NotSameAs(expectedPath).AsWildcard();
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that fileInfo
						              does not have the same content as file {Formatter.Format(fullExpectedPath)},
						              but it did match

						              File content:
						              bar
						              """);
				}
			}
		}
	}
}
