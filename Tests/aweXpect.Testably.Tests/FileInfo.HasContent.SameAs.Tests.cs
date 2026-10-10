using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed partial class HasContent
	{
		public class SameAs
		{
			public sealed class StringTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "baz");
					fileSystem.File.WriteAllText(expectedPath, "bar");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().SameAs(expectedPath);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that fileInfo
						              has the same content as file {Formatter.Format(fullExpectedPath)},
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
				public async Task WhenExpectedFileDoesNotExist_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "baz");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().SameAs(expectedPath);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that fileInfo
						              has the same content as file {Formatter.Format(fullExpectedPath)},
						              but it did not contain any file at {Formatter.Format(fullExpectedPath)}

						              File content:
						              baz
						              """);
				}

				[Fact]
				public async Task WhenContentMatches_ShouldSucceed()
				{
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string content = "bar";
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, content);
					fileSystem.File.WriteAllText(expectedPath, content);
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().SameAs(expectedPath);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					fileSystem.File.WriteAllText(expectedPath, "bar");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().SameAs(expectedPath);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that fileInfo
						              has the same content as file {Formatter.Format(fullExpectedPath)},
						              but it did not exist
						              """);
				}

				[Fact]
				public async Task WhenNegated_WhenFileDoesNotExist_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					fileSystem.File.WriteAllText(expectedPath, "bar");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).DoesNotComplyWith(it => it.HasContent().SameAs(expectedPath));
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that fileInfo
						              does not have the same content as file {Formatter.Format(fullExpectedPath)},
						              but it did not exist
						              """)
						.Because("a missing file has no content to compare, so the negation fails as well");
				}
			}

			public sealed class AsWildcardTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldFail()
				{
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					string fullExpectedPath = fileSystem.Path.GetFullPath(expectedPath);
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "baz");
					fileSystem.File.WriteAllText(expectedPath, "b?");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().SameAs(expectedPath).AsWildcard();
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that fileInfo
						              has the same content as file {Formatter.Format(fullExpectedPath)},
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
					MockFileSystem fileSystem = new();
					string path = "foo.txt";
					string expectedPath = "bar.txt";
					// ReSharper disable once MethodHasAsyncOverload
					fileSystem.File.WriteAllText(path, "bar");
					fileSystem.File.WriteAllText(expectedPath, "ba?");
					IFileInfo fileInfo = fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileInfo).HasContent().SameAs(expectedPath).AsWildcard();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
