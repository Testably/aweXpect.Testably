using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatFileInfo
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> is read-only.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo?>> IsReadOnly(this IThat<IFileInfo?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadOnlyConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> is not read-only.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo?>> IsNotReadOnly(this IThat<IFileInfo?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadOnlyConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsReadOnlyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<IFileInfo>(it, grammars),
			IValueConstraint<IFileInfo>
	{
		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			if (!actual.Exists)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = actual.IsReadOnly ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is read-only", "are read-only"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!Actual!.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" was not");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not read-only", "are not read-only"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!Actual!.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" was");
			}
		}
	}
}
