using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
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
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetLastWriteTimeUtc("foo", actual);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasLastWriteTime(expected);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that dirInfo
					              has last write time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(actual)}
					              """);
			}

			[Fact]
			public async Task WhenLastWriteTimeMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetLastWriteTimeUtc("foo", expected);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasLastWriteTime(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndLastWriteTimeMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetLastWriteTimeUtc("foo", expected);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotComplyWith(d => d.HasLastWriteTime(expected));
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that dirInfo
					              does not have last write time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(expected)}
					              """);
			}
		}
	}
}
