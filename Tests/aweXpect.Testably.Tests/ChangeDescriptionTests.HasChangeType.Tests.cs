using System.IO;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasChangeType
	{
		public sealed class Tests
		{
			[Test]
			public async Task DoesNotHaveChangeType_WhenChangeTypeDiffers_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveChangeType(WatcherChangeTypes.Deleted);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotHaveChangeType_WhenChangeTypeMatches_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveChangeType(WatcherChangeTypes.Created);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that change
					             does not have change type Created,
					             but it did
					             """);
			}

			[Test]
			public async Task DoesNotHaveChangeType_WhenUnexpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveChangeType(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected");
				await That(Act).Throws<ArgumentException>()
					.WithMessage("The unexpected change type must include at least one flag.*").AsWildcard();
			}

			[Test]
			public async Task WhenChangeTypeDiffers_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasChangeType(WatcherChangeTypes.Deleted);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that change
					             has change type Deleted,
					             but it was Created
					             """);
			}

			[Test]
			public async Task WhenChangeTypeMatches_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasChangeType(WatcherChangeTypes.Created);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasChangeType(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected");
				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected change type must include at least one flag.*").AsWildcard();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ChangeDescription? change = null;

				async Task Act()
				{
					await That(change!).HasChangeType(WatcherChangeTypes.Created);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that change
					             has change type Created,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedPartiallyOverlapsActual_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change)
						.HasChangeType(WatcherChangeTypes.Created | WatcherChangeTypes.Deleted);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that change
					             has change type Created | Deleted,
					             but it was Created
					             """);
			}
		}
	}
}
