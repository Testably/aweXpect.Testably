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
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IDriveInfo, IThat<IDriveInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new HasNameConstraint(it, grammars, s.options, s.expected)),
			source,
			options);
	}

	private sealed class HasNameConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithNotNullValue<IDriveInfo>(it, grammars),
			IAsyncConstraint<IDriveInfo>
	{
		private string? _actualName;

		public async ValueTask<ConstraintResult> IsMetBy(IDriveInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			_actualName = actual.Name;
			Outcome = await options.AreConsideredEqual(_actualName, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has name ", "have name ")).Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _actualName, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have name ", "do not have name "))
				.Append(options.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
