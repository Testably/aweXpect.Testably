using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
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
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetLastAccessTimeUtc("foo", actual);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasLastAccessTime(expected);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that dirInfo
					              has last access time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(actual)}
					              """);
			}

			[Fact]
			public async Task WhenLastAccessTimeMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetLastAccessTimeUtc("foo", expected);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasLastAccessTime(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndLastAccessTimeMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetLastAccessTimeUtc("foo", expected);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotComplyWith(d => d.HasLastAccessTime(expected));
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that dirInfo
					              does not have last access time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(expected)}
					              """);
			}
		}
	}
}
