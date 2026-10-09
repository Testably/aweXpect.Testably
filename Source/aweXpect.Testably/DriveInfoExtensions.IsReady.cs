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
	///     Verifies that the <see cref="IDriveInfo" /> is ready.
	/// </summary>
	public static AndOrResult<IDriveInfo, IThat<IDriveInfo>> IsReady(this IThat<IDriveInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadyConstraint(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the <see cref="IDriveInfo" /> is not ready.
	/// </summary>
	public static AndOrResult<IDriveInfo, IThat<IDriveInfo>> IsNotReady(this IThat<IDriveInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsReadyConstraint(it, grammars).Invert()),
			source);

	private sealed class IsReadyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IDriveInfo>(it, grammars),
			IValueConstraint<IDriveInfo>
	{
		public ConstraintResult IsMetBy(IDriveInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = actual.IsReady ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is ready");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was not");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not ready");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was");
			}
		}
	}
}
