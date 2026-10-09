using System.IO;
using System.IO.Abstractions;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DriveInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> <see cref="DriveType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDriveInfo, IThat<IDriveInfo>> HasDriveType(this IThat<IDriveInfo> source,
		DriveType expected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasDriveTypeConstraint(it, grammars, expected)),
			source);

	private sealed class HasDriveTypeConstraint(string it, ExpectationGrammars grammars, DriveType expected)
		: ConstraintResult.WithValue<IDriveInfo>(it, grammars),
			IValueConstraint<IDriveInfo>
	{
		private DriveType _actualDriveType;

		public ConstraintResult IsMetBy(IDriveInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualDriveType = actual.DriveType;
			Outcome = _actualDriveType == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has drive type ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ").Append(_actualDriveType);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have drive type ").Append(expected);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}
	}
}
