using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed partial class WithContent
		{
			public class NotSameAs
			{
				public sealed class StringTests
				{
					[Fact]
					public async Task WhenContentIsDifferent_ShouldSucceed()
					{
						MockFileSystem sut = new();
						string path = "foo.txt";
						string expectedPath = "bar.txt";
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, "baz");
						sut.File.WriteAllText(expectedPath, "bar");

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotSameAs(expectedPath);
						}

						await That(Act).DoesNotThrow();
					}

					[Fact]
					public async Task WhenContentMatches_ShouldFail()
					{
						MockFileSystem sut = new();
						string path = "foo.txt";
						string content = "bar";
						string expectedPath = "bar.txt";
						string fullExpectedPath = sut.Path.GetFullPath(expectedPath);
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, content);
						sut.File.WriteAllText(expectedPath, content);

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotSameAs(expectedPath);
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              has file '{path}' with not the same content as file '{fullExpectedPath}',
							              but it did match

							              File content:
							              bar
							              """);
					}

					[Fact]
					public async Task WhenNegated_WhenContentIsDifferent_ShouldFail()
					{
						MockFileSystem sut = new();
						string path = "foo.txt";
						string expectedPath = "bar.txt";
						string fullExpectedPath = sut.Path.GetFullPath(expectedPath);
						sut.File.WriteAllText(path, "baz");
						sut.File.WriteAllText(expectedPath, "bar");

						async Task Act()
						{
							await That(sut).DoesNotComplyWith(it => it.HasFile(path).WithContent().NotSameAs(expectedPath));
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              does not have file '{path}' with not the same content as file '{fullExpectedPath}',
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

				public sealed class AsWildcardTests
				{
					[Fact]
					public async Task WhenContentIsDifferent_ShouldSucceed()
					{
						MockFileSystem sut = new();
						string path = "foo.txt";
						string expectedPath = "bar.txt";
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, "baz");
						sut.File.WriteAllText(expectedPath, "b?");

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotSameAs(expectedPath).AsWildcard();
						}

						await That(Act).DoesNotThrow();
					}

					[Fact]
					public async Task WhenContentMatches_ShouldFail()
					{
						MockFileSystem sut = new();
						string path = "foo.txt";
						string expectedPath = "bar.txt";
						string fullExpectedPath = sut.Path.GetFullPath(expectedPath);
						// ReSharper disable once MethodHasAsyncOverload
						sut.File.WriteAllText(path, "bar");
						sut.File.WriteAllText(expectedPath, "ba?");

						async Task Act()
						{
							await That(sut).HasFile(path).WithContent().NotSameAs(expectedPath).AsWildcard();
						}

						await That(Act).Throws()
							.WithMessage($"""
							              Expected that sut
							              has file '{path}' with not the same content as file '{fullExpectedPath}',
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
