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
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> available free space in bytes.
	/// </summary>
	public static AndOrResult<IDriveInfo, IThat<IDriveInfo>> HasAvailableFreeSpace(this IThat<IDriveInfo> source,
		long expected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAvailableFreeSpaceConstraint(it, grammars, expected)),
			source);

	private sealed class HasAvailableFreeSpaceConstraint(string it, ExpectationGrammars grammars, long expected)
		: ConstraintResult.WithValue<IDriveInfo>(it, grammars),
			IValueConstraint<IDriveInfo>
	{
		private long _actualAvailableFreeSpace;

		public ConstraintResult IsMetBy(IDriveInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_actualAvailableFreeSpace = actual.AvailableFreeSpace;
			Outcome = _actualAvailableFreeSpace == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has available free space ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ").Append(_actualAvailableFreeSpace);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have available free space ").Append(expected);

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
