using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DriveInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> volume label.
	/// </summary>
	public static StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>> HasVolumeLabel(this IThat<IDriveInfo> source,
		string? expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasVolumeLabelConstraint(it, grammars, options, expected)),
			source,
			options);
	}

	private sealed class HasVolumeLabelConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string? expected)
		: ConstraintResult.WithValue<IDriveInfo>(it, grammars),
			IAsyncConstraint<IDriveInfo>
	{
		private string? _actualVolumeLabel;

		public async ValueTask<ConstraintResult> IsMetBy(IDriveInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualVolumeLabel = actual.VolumeLabel;
			Outcome = await options.AreConsideredEqual(_actualVolumeLabel, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has volume label ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _actualVolumeLabel, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have volume label ").Append(options.GetExpectation(expected, Grammars));

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
