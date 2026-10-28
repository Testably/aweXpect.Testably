using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatChangeDescription
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNameDiffers_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasName("bar.txt");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that change
					             has name equal to "bar.txt",
					             but it was "foo.txt", which differs at index 0:
					                ↓ (actual)
					               "foo.txt"
					               "bar.txt"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenNameMatches_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasName(change.Name);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndNameMatches_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotComplyWith(c => c.HasName(change.Name));
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that change
					             does not have name equal to "foo.txt",
					             but it did
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ChangeDescription? change = null;

				async Task Act()
				{
					await That(change!).HasName("foo.txt");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that change
					             has name equal to "foo.txt",
					             but it was <null>
					             """);
			}
		}
	}
}
