using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class HasCreationTime
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenCreationTimeDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				DateTime actual = expected.AddSeconds(1);
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetCreationTimeUtc("foo", actual);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasCreationTime(expected);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that dirInfo
					              has creation time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(actual)}
					              """);
			}

			[Fact]
			public async Task WhenCreationTimeMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetCreationTimeUtc("foo", expected);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasCreationTime(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedAndCreationTimeMatches_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				DateTime expected = CurrentTime().ToUniversalTime();
				fileSystem.Directory.CreateDirectory("foo");
				fileSystem.Directory.SetCreationTimeUtc("foo", expected);
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).DoesNotComplyWith(d => d.HasCreationTime(expected));
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that dirInfo
					              does not have creation time equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(expected)}
					              """);
			}
		}
	}
}
