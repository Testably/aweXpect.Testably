using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileSystem
{
	public sealed partial class HasDirectory
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenDirectoryExists_ShouldSucceed()
			{
				string path = "foo";
				MockFileSystem sut = new();
				sut.Directory.CreateDirectory(path);

				async Task Act()
				{
					await That(sut).HasDirectory(path);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDirectoryIsMissing_ShouldFail()
			{
				string path = "foo";
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).HasDirectory(path);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that sut
					              has directory {Formatter.Format(path)},
					              but it did not exist
					              """);
			}

			[Test]
			public async Task WhenPathIsFile_ShouldFail()
			{
				string path = "foo";
				MockFileSystem sut = new();
				// ReSharper disable once MethodHasAsyncOverload
				sut.File.WriteAllText(path, "");

				async Task Act()
				{
					await That(sut).HasDirectory(path);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that sut
					              has directory {Formatter.Format(path)},
					              but it was a file
					              """);
			}
		}
	}
}
