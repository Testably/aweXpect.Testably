using System.IO;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ThatChangeDescription
{
	public sealed class HasNotifyFilters
	{
		public sealed class Tests
		{
			[Test]
			public async Task DoesNotHaveNotifyFilters_WhenFlagIsMissing_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveNotifyFilters(NotifyFilters.Security);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotHaveNotifyFilters_WhenFlagIsPresent_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));
				NotifyFilters present = change.NotifyFilters;

				async Task Act()
				{
					await That(change).DoesNotHaveNotifyFilters(present);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that change
					              does not have notify filters {present},
					              but it did
					              """);
			}

			[Test]
			public async Task DoesNotHaveNotifyFilters_WhenUnexpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveNotifyFilters(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected");
				await That(Act).Throws<ArgumentException>()
					.WithMessage("The unexpected notify filters must include at least one flag.*").AsWildcard();
			}

			[Test]
			public async Task WhenContainingExpectedFlag_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));
				NotifyFilters expected = change.NotifyFilters;

				async Task Act()
				{
					await That(change).HasNotifyFilters(expected);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsDefault_ShouldThrowArgumentException()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).HasNotifyFilters(default);
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected");
				await That(Act).Throws<ArgumentException>()
					.WithMessage("The expected notify filters must include at least one flag.*").AsWildcard();
			}

			[Test]
			public async Task WhenExpectedPartiallyOverlapsActual_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));
				NotifyFilters present = change.NotifyFilters;

				async Task Act()
				{
					await That(change).HasNotifyFilters(present | NotifyFilters.Security);
				}

				await That(Act).Throws()
					.WithMessage("*has notify filters*Security*but it was*").AsWildcard();
			}

			[Test]
			public async Task WhenMissingExpectedFlag_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				NotifyFilters actual = change.NotifyFilters;

				async Task Act()
				{
					await That(change).HasNotifyFilters(NotifyFilters.Security);
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that change
					              has notify filters Security,
					              but it was {actual}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ChangeDescription? change = null;

				async Task Act()
				{
					await That(change!).HasNotifyFilters(NotifyFilters.FileName);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that change
					             has notify filters FileName,
					             but it was <null>
					             """);
			}
		}
	}
}
