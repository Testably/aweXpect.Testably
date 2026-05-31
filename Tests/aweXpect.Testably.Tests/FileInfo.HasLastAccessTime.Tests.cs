using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileInfo
{
	public sealed class HasLastAccessTime
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenLastAccessTimeDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				DateTime actual = expected.AddSeconds(1);
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetLastAccessTimeUtc(path, actual);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasLastAccessTime(expected);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that fileInfo
					              has last access time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(actual)}
					              """);
			}

			[Fact]
			public async Task WhenLastAccessTimeMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetLastAccessTimeUtc(path, expected);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).HasLastAccessTime(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndLastAccessTimeMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				string path = "foo.txt";
				fileSystem.File.WriteAllText(path, "");
				fileSystem.File.SetLastAccessTimeUtc(path, expected);
				IFileInfo fileInfo = fileSystem.FileInfo.New(path);

				async Task Act()
				{
					await That(fileInfo).DoesNotComplyWith(f => f.HasLastAccessTime(expected));
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that fileInfo
					              does not have last access time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(expected)}
					              """);
			}
		}
	}
}
