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
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> total free space in bytes.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDriveInfo, IThat<IDriveInfo>> HasTotalFreeSpace(this IThat<IDriveInfo> source,
		long expected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasTotalFreeSpaceConstraint(it, grammars, expected)),
			source);

	private sealed class HasTotalFreeSpaceConstraint(string it, ExpectationGrammars grammars, long expected)
		: ConstraintResult.WithValue<IDriveInfo>(it, grammars),
			IValueConstraint<IDriveInfo>
	{
		private long _actualTotalFreeSpace;

		public ConstraintResult IsMetBy(IDriveInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualTotalFreeSpace = actual.TotalFreeSpace;
			Outcome = _actualTotalFreeSpace == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has total free space ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ").Append(_actualTotalFreeSpace);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have total free space ").Append(expected);

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
