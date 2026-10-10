using Testably.Abstractions.Testing;

namespace aweXpect.Testably.Tests;

public sealed partial class Statistics
{
	public sealed partial class Recorded
	{
		public sealed class Predicates
		{
			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(fileSystem.Statistics).Recorded()
						.File.WriteAllText(path: _ => throw new InvalidOperationException("boom")).Once();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has recorded a call to File.WriteAllText with path matching _ => throw new InvalidOperationException("boom") exactly once,
					             but the path predicate did throw an InvalidOperationException:
					               boom
					             """);
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldAlsoFailWithNever()
			{
				MockFileSystem fileSystem = new();
				fileSystem.File.WriteAllText("foo.txt", "x");

				async Task Act()
				{
					await That(fileSystem.Statistics).Recorded()
						.File.WriteAllText(path: _ => throw new InvalidOperationException("boom")).Never();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fileSystem.Statistics
					             has recorded no call to File.WriteAllText with path matching _ => throw new InvalidOperationException("boom"),
					             but the path predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("a predicate that throws answers nothing, so it must not count as a non-matching call");
			}
		}
	}
}
