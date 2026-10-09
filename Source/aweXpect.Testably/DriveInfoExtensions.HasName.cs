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
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>> HasName(this IThat<IDriveInfo> source,
		string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasNameConstraint(it, grammars, options, expected)),
			source,
			options);
	}

	private sealed class HasNameConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithValue<IDriveInfo>(it, grammars),
			IAsyncConstraint<IDriveInfo>
	{
		private string? _actualName;

		public async ValueTask<ConstraintResult> IsMetBy(IDriveInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualName = actual.Name;
			Outcome = await options.AreConsideredEqual(_actualName, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _actualName, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have name ")
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
