using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class DirectoryNegated
		{
			[Fact]
			public async Task WhenNegatingNeverAndCalled_ShouldSucceed()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().Directory.CreateDirectory().Never());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatingNeverAndNotCalled_ShouldFailWithAtLeastOneWording()
			{
				MockFileSystem fileSystem = new();

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().Directory.CreateDirectory().Never());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             recorded at least one call to Directory.CreateDirectory,
					             but it was recorded 0 times
					             """);
			}

			[Fact]
			public async Task WhenNegatingNeverWithMatcherAndNotCalled_ShouldIncludeMatcher()
			{
				MockFileSystem fileSystem = new();

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().Directory.CreateDirectory(p => p == "foo").Never());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             recorded at least one call to Directory.CreateDirectory with path matching p => p == "foo",
					             but it was recorded 0 times
					             """);
			}

			[Fact]
			public async Task WhenNegatingExactlyAndMatching_ShouldFailWithDidNotRecordWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().Directory.CreateDirectory().Once());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             did not record a call to Directory.CreateDirectory exactly once,
					             but it was recorded 1 time
					             """);
			}
		}
	}
}
