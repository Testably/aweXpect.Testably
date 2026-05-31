using System.IO;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Tests;

public sealed partial class ChangeDescriptionTests
{
	public sealed class HasNotifyFilters
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotHaveNotifyFilters_WhenFlagIsMissing_ShouldSucceed()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				async Task Act()
				{
					await That(change).DoesNotHaveNotifyFilters(NotifyFilters.Security);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHaveNotifyFilters_WhenFlagIsPresent_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));
				NotifyFilters present = change.NotifyFilters;

				async Task Act()
				{
					await That(change).DoesNotHaveNotifyFilters(present);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that change
					              does not have notify filters {present},
					              but it did
					              """);
			}

			[Fact]
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

			[Fact]
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

			[Fact]
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

			[Fact]
			public async Task WhenMissingExpectedFlag_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));

				NotifyFilters actual = change.NotifyFilters;

				async Task Act()
				{
					await That(change).HasNotifyFilters(NotifyFilters.Security);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that change
					              has notify filters Security,
					              but it was {actual}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ChangeDescription? change = null;

				async Task Act()
				{
					await That(change!).HasNotifyFilters(NotifyFilters.FileName);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that change
					             has notify filters FileName,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedPartiallyOverlapsActual_ShouldFail()
			{
				ChangeDescription change = Capture(fs => fs.File.WriteAllText("foo.txt", ""));
				NotifyFilters present = change.NotifyFilters;

				async Task Act()
				{
					await That(change).HasNotifyFilters(present | NotifyFilters.Security);
				}

				await That(Act).ThrowsException()
					.WithMessage("*has notify filters*Security*but it was*").AsWildcard();
			}
		}
	}
}
