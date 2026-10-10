using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed partial class FileInfo
		{
			public sealed class FactoryTests
			{
				[Fact]
				public async Task New_WithFileNameFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo.New(f => f == "foo.txt").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to FileInfo.New with fileName matching f => f == "foo.txt" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WhenFactoryCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					fileSystem.FileInfo.New("foo.txt");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().FileInfo.New().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Wrap_WithFileInfoFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.FileInfo.Wrap(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to FileInfo.Wrap with fileInfo matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}
			}
		}
	}
}
