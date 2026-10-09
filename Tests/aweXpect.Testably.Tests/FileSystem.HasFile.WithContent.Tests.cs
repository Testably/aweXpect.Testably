using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed partial class WithContent
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldFail()
				{
					string path = "foo.txt";
					MockFileSystem sut = new();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent("bar");
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with content equal to "bar",
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
					MockFileSystem sut = new();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, content);

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent(content);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					string path = "foo.txt";
					MockFileSystem sut = new();

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent("bar");
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with content equal to "bar",
						              but it did not exist
						              """)
						.Because("a missing file has no content to read, so it fails instead of throwing");
				}

				[Fact]
				public async Task WhenNegated_WhenContentMatches_ShouldFail()
				{
					string path = "foo.txt";
					MockFileSystem sut = new();
					sut.File.WriteAllText(path, "bar");

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithContent("bar"));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              does not have file '{path}' with content equal to "bar",
						              but it did and did match

						              File content:
						              bar
						              """)
						.Because("the negation belongs to the verb only");
				}

				[Fact]
				public async Task WhenNegated_WhenFileDoesNotExist_ShouldSucceed()
				{
					MockFileSystem sut = new();

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile("foo.txt").WithContent("bar"));
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPathIsADirectory_ShouldFail()
				{
					string path = "foo";
					MockFileSystem sut = new();
					sut.Directory.CreateDirectory(path);

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent("bar");
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with content equal to "bar",
						              but it was a directory
						              """);
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
					MockFileSystem sut = new();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllBytes(path, content);

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent(expected);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with content equal to expected,
						              but it differed
						              """);
				}

				[Fact]
				public async Task WhenContentMatches_ShouldSucceed()
				{
					byte[] content = Encoding.UTF8.GetBytes("baz");
					string path = "foo.txt";
					MockFileSystem sut = new();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllBytes(path, content);

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent(content);
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenFileDoesNotExist_ShouldFail()
				{
					byte[] expected = Encoding.UTF8.GetBytes("bar");
					string path = "foo.txt";
					MockFileSystem sut = new();

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent(expected);
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with content equal to expected,
						              but it did not exist
						              """)
						.Because("a missing file has no content to read, so it fails instead of throwing");
				}

				[Fact]
				public async Task WhenNegated_WhenContentMatches_ShouldFail()
				{
					byte[] content = Encoding.UTF8.GetBytes("baz");
					string path = "foo.txt";
					MockFileSystem sut = new();
					sut.File.WriteAllBytes(path, content);

					async Task Act()
					{
						await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithContent(content));
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              does not have file '{path}' with content equal to content,
						              but it did and did match
						              """)
						.Because("the negation belongs to the verb only");
				}
			}

			public sealed class AsWildcardTests
			{
				[Fact]
				public async Task WhenContentIsDifferent_ShouldFail()
				{
					string path = "foo.txt";
					MockFileSystem sut = new();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "baz");

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent("b?").AsWildcard();
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that sut
						              has file '{path}' with content matching "b?",
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
					MockFileSystem sut = new();
					// ReSharper disable once MethodHasAsyncOverload
					sut.File.WriteAllText(path, "bar");

					async Task Act()
					{
						await That(sut).HasFile(path).WithContent("ba?").AsWildcard();
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
