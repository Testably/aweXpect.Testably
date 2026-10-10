using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed partial class DirectoryInfo
		{
			public sealed class NewTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DirectoryInfo.New("foo");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().DirectoryInfo.New().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPathFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo.New(p => p == "foo").Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to DirectoryInfo.New with path matching p => p == "foo" exactly once,
						             but it was recorded 0 times
						             """);
				}

				[Fact]
				public async Task WithPathFilter_ShouldOnlyCountMatching()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DirectoryInfo.New("foo");
					_ = fileSystem.DirectoryInfo.New("bar");

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo.New(p => p == "foo").Once();
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WrapTests
			{
				[Fact]
				public async Task WhenCalled_ShouldRecord()
				{
					MockFileSystem fileSystem = new();
					_ = fileSystem.DirectoryInfo.Wrap(new System.IO.DirectoryInfo("foo"));

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded().DirectoryInfo.Wrap().Once();
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithDirectoryInfoFilter_NoMatch_ShouldFailWithMessage()
				{
					MockFileSystem fileSystem = new();

					async Task Act()
					{
						await That(fileSystem.Statistics).Recorded()
							.DirectoryInfo.Wrap(_ => true).Once();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that fileSystem.Statistics
						             has recorded a call to DirectoryInfo.Wrap with directoryInfo matching _ => true exactly once,
						             but it was recorded 0 times
						             """);
				}
			}
		}
	}
}
