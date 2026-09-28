using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class FileSystem
{
	public sealed partial class HasFile
	{
		public sealed class Which
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ChainedAnd_HasContent_ShouldSucceed()
				{
					MockFileSystem sut = new();
					sut.Initialize().WithFile("a.txt").Which(f => f.HasStringContent("hello"));

					async Task Act()
					{
						await That(sut).HasFile("a.txt")
							.Which.HasLength(5).And.HasContent("hello");
					}

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task HasLength_WhenLengthDiffers_ShouldFail()
				{
					MockFileSystem sut = new();
					sut.Initialize().WithFile("a.txt").Which(f => f.HasStringContent("hello"));

					async Task Act()
					{
						await That(sut).HasFile("a.txt").Which.HasLength(99);
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that sut
						             has file 'a.txt' which has length 99,
						             but it was 5
						             """);
				}

				[Fact]
				public async Task HasLength_WhenLengthMatches_ShouldSucceed()
				{
					MockFileSystem sut = new();
					sut.Initialize().WithFile("a.txt").Which(f => f.HasStringContent("hello"));

					async Task Act()
					{
						await That(sut).HasFile("a.txt").Which.HasLength(5);
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
