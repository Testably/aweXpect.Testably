using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class DirectoryTwoMatchers
		{
#if NET8_0_OR_GREATER
			[Fact]
			public async Task WithTwoMatchers_NoMatch_ShouldJoinMatchersWithComma()
			{
				MockFileSystem fileSystem = new();

				async Task Act()
				{
					await That(fileSystem.Statistics).Recorded()
						.Directory.CreateSymbolicLink(p => p == "foo", t => t == "bar").Once();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             recorded a call to Directory.CreateSymbolicLink with path matching p => p == "foo", pathToTarget matching t => t == "bar" exactly once,
					             but it was recorded 0 times
					             """);
			}

			[Fact]
			public async Task WithNeverAndMatcher_Matching_ShouldFailIncludingMatcher()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("target");
				fileSystem.Directory.CreateSymbolicLink("link", "target");

				async Task Act()
				{
					await That(fileSystem.Statistics).Recorded()
						.Directory.CreateSymbolicLink(p => p == "link").Never();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             recorded no call to Directory.CreateSymbolicLink with path matching p => p == "link",
					             but it was recorded 1 time
					             """);
			}
#endif
		}
	}
}
