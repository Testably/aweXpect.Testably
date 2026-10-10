using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed class DoesNotHaveFile
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFileExists_ShouldFail()
			{
				string path = "foo";
				MockFileSystem sut = new();
				// ReSharper disable once MethodHasAsyncOverload
				sut.File.WriteAllText(path, "");

				async Task Act()
				{
					await That(sut).DoesNotHaveFile(path);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that sut
					              does not have file {Formatter.Format(path)},
					              but it did
					              """);
			}

			[Fact]
			public async Task WhenFileIsMissing_ShouldSucceed()
			{
				string path = "foo";
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).DoesNotHaveFile(path);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPathIsDirectory_ShouldSucceed()
			{
				string path = "foo";
				MockFileSystem sut = new();
				sut.Directory.CreateDirectory(path);

				async Task Act()
				{
					await That(sut).DoesNotHaveFile(path);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
