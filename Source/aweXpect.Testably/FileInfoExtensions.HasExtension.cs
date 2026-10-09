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

public static partial class FileInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> has the <paramref name="expected" /> extension.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> HasExtension(this IThat<IFileInfo> source,
		string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasExtensionConstraint(it, grammars, options, expected)),
			source,
			options);
	}

	private sealed class HasExtensionConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithNotNullValue<IFileInfo>(it, grammars),
			IAsyncConstraint<IFileInfo>
	{
		private string? _actualExtension;

		public async ValueTask<ConstraintResult> IsMetBy(IFileInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			_actualExtension = actual.Extension;
			Outcome = await options.AreConsideredEqual(_actualExtension, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has extension ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _actualExtension, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have extension ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
