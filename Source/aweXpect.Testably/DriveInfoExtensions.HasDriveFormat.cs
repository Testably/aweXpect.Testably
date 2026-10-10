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
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> drive format.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>> HasDriveFormat(this IThat<IDriveInfo> source,
		string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasDriveFormatConstraint(it, grammars, options, expected)),
			source,
			options);
	}

	private sealed class HasDriveFormatConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithValue<IDriveInfo>(it, grammars),
			IAsyncConstraint<IDriveInfo>
	{
		private string? _actualDriveFormat;

		public async ValueTask<ConstraintResult> IsMetBy(IDriveInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualDriveFormat = actual.DriveFormat;
			Outcome = await options.AreConsideredEqual(_actualDriveFormat, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has drive format ", "have drive format ")).Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _actualDriveFormat, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have drive format ", "do not have drive format "))
				.Append(options.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

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
