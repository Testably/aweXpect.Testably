using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatDriveInfo
{
	/// <summary>
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> drive format.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo?>> HasDriveFormat(this IThat<IDriveInfo?> subject,
		string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new HasDriveFormatConstraint(it, grammars, s.options, s.expected)),
			subject,
			options);
	}

	private sealed class HasDriveFormatConstraint(
		string it,
		ExpectationGrammars grammars,
		OptionsHolder<StringEqualityOptions> options,
		string expected)
		: ConstraintResult.WithNotNullValue<IDriveInfo>(it, grammars),
			IAsyncConstraint<IDriveInfo>
	{
		private MatchResult _matchResult;
		private string? _actualDriveFormat;

		public async ValueTask<ConstraintResult> IsMetBy(IDriveInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			_actualDriveFormat = actual.DriveFormat;
			_matchResult = await options.Value.MatchWithExplanation(_actualDriveFormat, expected, nameof(expected));
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has drive format ", "have drive format ")).Append(options.Value.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.Value.GetExtendedFailure(It,Grammars, _actualDriveFormat, expected, _matchResult));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have drive format ", "do not have drive format "))
				.Append(options.Value.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
