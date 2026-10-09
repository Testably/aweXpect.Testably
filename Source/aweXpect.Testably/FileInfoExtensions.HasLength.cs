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
	///     Verifies that the <see cref="IFileInfo" /> has the <paramref name="expected" /> length in bytes.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo>> HasLength(this IThat<IFileInfo> source,
		long expected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasLengthConstraint(it, grammars, expected)),
			source);

	private sealed class HasLengthConstraint(string it, ExpectationGrammars grammars, long expected)
		: ConstraintResult.WithValue<IFileInfo>(it, grammars),
			IValueConstraint<IFileInfo>
	{
		private long _actualLength;

		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			Actual = actual;
			if (actual is null || !actual.Exists)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualLength = actual.Length;
			Outcome = _actualLength == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has length ", "have length ")).Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else if (!Actual.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ").Append(_actualLength);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have length ", "do not have length ")).Append(expected);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else if (!Actual.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}
	}
}
