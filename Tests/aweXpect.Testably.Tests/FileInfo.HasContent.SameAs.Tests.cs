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

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that fileInfo
						              has the same content as file '{fullExpectedPath}',
						              but it was "baz" which differs at index 2:
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

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that fileInfo
						              has the same content as file '{fullExpectedPath}',
						              but it did not contain any file at '{fullExpectedPath}'

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

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that fileInfo
						              has the same content as file '{fullExpectedPath}',
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
