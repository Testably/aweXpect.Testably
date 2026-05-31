using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class HasLastWriteTime
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenLastWriteTimeDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				DateTime actual = expected.AddSeconds(1);
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetLastWriteTimeUtc(path, actual);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasLastWriteTime(expected);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that fileInfo
					              has last write time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(actual)}
					              """);
			}

			[Fact]
			public async Task WhenLastWriteTimeMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetLastWriteTimeUtc(path, expected);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasLastWriteTime(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndLastWriteTimeMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetLastWriteTimeUtc(path, expected);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(f => f.HasLastWriteTime(expected));
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that fileInfo
					              does not have last write time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(expected)}
					              """);
			}
		}
	}
}
