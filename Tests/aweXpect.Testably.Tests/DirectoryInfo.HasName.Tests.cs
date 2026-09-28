using System.IO.Abstractions;
using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class DirectoryInfo
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNameDiffers_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasName("bar");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that dirInfo
					             has name equal to "bar",
					             but it was "foo", which differs at index 0:
					                ↓ (actual)
					               "foo"
					               "bar"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenNameMatches_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");
				IDirectoryInfo dirInfo = fileSystem.DirectoryInfo.New("foo");

				async Task Act()
				{
					await That(dirInfo).HasName("foo");
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
