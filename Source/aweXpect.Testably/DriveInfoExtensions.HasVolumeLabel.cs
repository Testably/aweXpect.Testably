using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Testably;

public static partial class DriveInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDriveInfo" /> has the <paramref name="expected" /> volume label.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>> HasVolumeLabel(this IThat<IDriveInfo> subject,
		string? expected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>>(
			subject.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new HasVolumeLabelConstraint(it, grammars, s.options, s.expected)),
			subject,
			options);
	}

	private sealed class HasVolumeLabelConstraint(
		string it,
		ExpectationGrammars grammars,
		OptionsHolder<StringEqualityOptions> options,
		string? expected)
		: ConstraintResult.WithNotNullValue<IDriveInfo>(it, grammars),
			IAsyncConstraint<IDriveInfo>
	{
		private MatchResult _matchResult;
		private string? _actualVolumeLabel;

		public async ValueTask<ConstraintResult> IsMetBy(IDriveInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			_actualVolumeLabel = actual.VolumeLabel;
			_matchResult = await options.Value.MatchWithExplanation(_actualVolumeLabel, expected, nameof(expected));
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has volume label ", "have volume label ")).Append(options.Value.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.Value.GetExtendedFailure(It,Grammars, _actualVolumeLabel, expected, _matchResult));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have volume label ", "do not have volume label "))
				.Append(options.Value.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
