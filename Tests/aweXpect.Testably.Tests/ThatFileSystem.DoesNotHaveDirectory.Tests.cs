using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileSystem
{
	public sealed class DoesNotHaveDirectory
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenDirectoryExists_ShouldFail()
			{
				string path = "foo";
				MockFileSystem sut = new();
				sut.Directory.CreateDirectory(path);

				async Task Act()
				{
					await That(sut).DoesNotHaveDirectory(path);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that sut
					              does not have directory {Formatter.Format(path)},
					              but it did
					              """);
			}

			[Test]
			public async Task WhenDirectoryIsMissing_ShouldSucceed()
			{
				string path = "foo";
				MockFileSystem sut = new();

				async Task Act()
				{
					await That(sut).DoesNotHaveDirectory(path);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPathIsFile_ShouldSucceed()
			{
				string path = "foo";
				MockFileSystem sut = new();
				// ReSharper disable once MethodHasAsyncOverload
				sut.File.WriteAllText(path, "");

				async Task Act()
				{
					await That(sut).DoesNotHaveDirectory(path);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
