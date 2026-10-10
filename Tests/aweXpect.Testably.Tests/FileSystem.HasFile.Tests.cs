using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFileExists_ShouldSucceed()
			{
				string path = "foo";
				MockFileSystem sut = new();
				// ReSharper disable once MethodHasAsyncOverload
				sut.File.WriteAllText(path, "");

				async Task Act()
				{
					await That(sut).HasFile(path);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFileIsMissing_ShouldFail()
			{
				string path = "foo";
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).HasFile(path);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that sut
					              has file {Formatter.Format(path)},
					              but it did not exist
					              """);
			}

			[Fact]
			public async Task WhenPathIsDirectory_ShouldFail()
			{
				string path = "foo";
				MockFileSystem sut = new();
				sut.Directory.CreateDirectory(path);

				async Task Act()
				{
					await That(sut).HasFile(path);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that sut
					              has file {Formatter.Format(path)},
					              but it was a directory
					              """);
			}
		}
	}
}
