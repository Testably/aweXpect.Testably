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
							              has file '{path}' with content different from content,
							              but it did match
							              """);
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
							              has file '{path}' with content not equal to "bar",
							              but it did match

							              File content:
							              bar
							              """);
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
							              has file '{path}' with content not matching "ba?",
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
