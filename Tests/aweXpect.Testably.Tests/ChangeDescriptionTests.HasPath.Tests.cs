using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasPath
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPathDiffers_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasPath("totally-different-path");
				}

				await That(Act).Throws()
					.WithMessage("*has path equal to \"totally-different-path\"*").AsWildcard();
			}

			[Fact]
			public async Task WhenPathMatches_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasPath(change.Path);
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
