using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed partial class HasContent
	{
		public class NotSameAs
		{
			public sealed class StringTests
			{
				[Fact]
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

				[Fact]
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
						              does not have the same content as file '{fullExpectedPath}',
						              but it did match

						              File content:
						              bar
						              """);
				}
			}

			public sealed class AsWildcardTests
			{
				[Fact]
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

				[Fact]
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
						              does not have the same content as file '{fullExpectedPath}',
						              but it did match

						              File content:
						              bar
						              """);
				}
			}
		}
	}
}
