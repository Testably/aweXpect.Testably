using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed partial class WithContent
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
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllBytes(path, content);

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotEqualTo(expected);
						}

						await That(Act).DoesNotThrow();
					}

					[Fact]
					public async Task WhenContentMatches_ShouldFail()
					{
						string path = "foo.txt";
						byte[] content = Encoding.UTF8.GetBytes("baz");
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllBytes(path, content);

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotEqualTo(content);
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              has file {Formatter.Format(path)} with content different from content,
							              but it did match
							              """);
					}

					[Fact]
					public async Task WhenFileDoesNotExist_ShouldFail()
					{
						byte[] unexpected = Encoding.UTF8.GetBytes("bar");
						string path = "foo.txt";
						MockFileSystem sut = new();

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotEqualTo(unexpected);
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              has file {Formatter.Format(path)} with content different from unexpected,
							              but it did not exist
							              """)
							.Because("a missing file has no content to read, so it fails instead of throwing");
					}

					[Fact]
					public async Task WhenNegated_WhenContentIsDifferent_ShouldFail()
					{
						byte[] content = Encoding.UTF8.GetBytes("baz");
						byte[] unexpected = Encoding.UTF8.GetBytes("bar");
						string path = "foo.txt";
						MockFileSystem sut = new();
						sut.File.WriteAllBytes(path, content);

						async Task Act()
						{
							await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithContent().NotEqualTo(unexpected));
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              does not have file {Formatter.Format(path)} with content different from unexpected,
							              but it did and differed
							              """)
							.Because("the outer negation belongs to the verb, while the content clause keeps its own negation");
					}
				}

				public sealed class StringTests
				{
					[Fact]
					public async Task WhenContentIsDifferent_ShouldSucceed()
					{
						string path = "foo.txt";
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, "baz");

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotEqualTo("bar");
						}

						await That(Act).DoesNotThrow();
					}

					[Fact]
					public async Task WhenContentMatches_ShouldFail()
					{
						string path = "foo.txt";
						string content = "bar";
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, content);

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotEqualTo(content);
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              has file {Formatter.Format(path)} with content not equal to "bar",
							              but it did match

							              File content:
							              bar
							              """);
					}

					[Fact]
					public async Task WhenFileDoesNotExist_ShouldFail()
					{
						string path = "foo.txt";
						MockFileSystem sut = new();

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotEqualTo("bar");
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              has file {Formatter.Format(path)} with content not equal to "bar",
							              but it did not exist
							              """)
							.Because("a missing file has no content to read, so it fails instead of throwing");
					}

					[Fact]
					public async Task WhenNegated_WhenContentIsDifferent_ShouldFail()
					{
						string path = "foo.txt";
						MockFileSystem sut = new();
						sut.File.WriteAllText(path, "baz");

						async Task Act()
						{
							await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithContent().NotEqualTo("bar"));
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              does not have file {Formatter.Format(path)} with content not equal to "bar",
							              but it did and was "baz", which differs at index 2:
							                   ↓ (actual)
							                "baz"
							                "bar"
							                   ↑ (expected)

							              File content:
							              baz
							              """)
							.Because("the outer negation belongs to the verb, while the content clause keeps its own negation");
					}
				}

				public sealed class WildcardTests
				{
					[Fact]
					public async Task WhenContentIsDifferent_ShouldSucceed()
					{
						string path = "foo.txt";
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, "baz");

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotEqualTo("b?").AsWildcard();
						}

						await That(Act).DoesNotThrow();
					}

					[Fact]
					public async Task WhenContentMatches_ShouldFail()
					{
						string path = "foo.txt";
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, "bar");

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotEqualTo("ba?").AsWildcard();
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              has file {Formatter.Format(path)} with content not matching "ba?",
							              but it did match

							              File content:
							              bar
							              """);
					}
				}
			}
		}
	}
}
