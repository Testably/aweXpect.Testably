using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> is read-only.
	/// </summary>
	public static AndOrResult<IFileInfo, IThat<IFileInfo>> IsReadOnly(this IThat<IFileInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadOnlyConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> is not read-only.
	/// </summary>
	public static AndOrResult<IFileInfo, IThat<IFileInfo>> IsNotReadOnly(this IThat<IFileInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadOnlyConstraint(it, grammars).Invert()),
			source);

	private sealed class IsReadOnlyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IFileInfo>(it, grammars),
			IValueConstraint<IFileInfo>
	{
		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			Actual = actual;
			if (!actual.Exists)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = actual.IsReadOnly ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is read-only");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Exists != true)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" was not");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not read-only");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.Exists != true)
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
