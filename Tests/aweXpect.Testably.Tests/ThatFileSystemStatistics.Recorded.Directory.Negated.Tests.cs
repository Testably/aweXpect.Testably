using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatFileSystemStatistics
{
	public sealed partial class Recorded
	{
		public sealed class DirectoryNegated
		{
			[Test]
			public async Task WhenNegatingExactlyAndMatching_ShouldFailWithDidNotRecordWording()
			{
				MockFileSystem fileSystem = new();
				fileSystem.Directory.CreateDirectory("foo");

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().Directory.CreateDirectory().Once());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has not recorded a call to Directory.CreateDirectory exactly once,
					             but it was recorded 1 time
					             """);
			}

			[Test]
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

			[Test]
			public async Task WhenNegatingNeverAndNotCalled_ShouldFailWithAtLeastOneWording()
			{
				MockFileSystem fileSystem = new();

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().Directory.CreateDirectory().Never());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has recorded at least one call to Directory.CreateDirectory,
					             but it was recorded 0 times
					             """);
			}

			[Test]
			public async Task WhenNegatingNeverWithMatcherAndNotCalled_ShouldIncludeMatcher()
			{
				MockFileSystem fileSystem = new();

				async Task Act()
				{
					await That(fileSystem.Statistics).DoesNotComplyWith(it
						=> it.Recorded().Directory.CreateDirectory(p => p == "foo").Never());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has recorded at least one call to Directory.CreateDirectory with path matching p => p == "foo",
					             but it was recorded 0 times
					             """);
			}
		}
	}
}
