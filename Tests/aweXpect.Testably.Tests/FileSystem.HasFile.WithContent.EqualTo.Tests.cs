using System.Text;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed partial class WithContent
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
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllBytes(path, content);

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().EqualTo(expected);
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
							await That(sut).HasFile(path).WithContent().EqualTo(content);
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
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, "baz");

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().EqualTo("bar");
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
							await That(sut).HasFile(path).WithContent().EqualTo(content);
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
						MockFileSystem sut = new();
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, "baz");

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().EqualTo("b?").AsWildcard();
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
							await That(sut).HasFile(path).WithContent().EqualTo("ba?").AsWildcard();
						}

						await That(Act).DoesNotThrow();
					}
				}
			}
		}
	}
}
