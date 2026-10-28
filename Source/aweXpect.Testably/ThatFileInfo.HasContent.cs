using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;

namespace aweXpect;

public static partial class ThatFileInfo
{
	/// <summary>
	///     Verifies that the string content of the <see cref="IFileInfo" /> is equal to
	///     the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IFileInfo, IThat<IFileInfo?>> HasContent(this IThat<IFileInfo?> subject,
		string? expected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((expected, options), static (s, it, grammars)
				=> new HasContentValueConstraint(
					it, grammars, s.expected, s.options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the content of the <see cref="IFileInfo" /> is equal to
	///     the <paramref name="expected" /> binary content.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo?>> HasContent(this IThat<IFileInfo?> subject,
		byte[] expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		return new AndOrResult<IFileInfo, IThat<IFileInfo?>>(subject.Get().ExpectationBuilder.AddConstraint((expected, doNotPopulateThisValue), static (s, it, grammars)
				=> new HasBinaryContentConstraint(
					it, grammars, s.expected, s.doNotPopulateThisValue)),
			subject);
	}

	/// <summary>
	///     Verifies that the content of the <see cref="IFileInfo" />…
	/// </summary>
	[GuaranteesNotNull]
	public static FileInfoContentResult HasContent(this IThat<IFileInfo?> subject)
		=> new(subject.Get().ExpectationBuilder, subject);

	private sealed class HasContentValueConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		OptionsHolder<StringEqualityOptions> options)
		: ConstraintResult.WithNotNullValue<IFileInfo>(it, grammars),
			IAsyncConstraint<IFileInfo>
	{
		private MatchResult _matchResult;
		private string? _fileContent;

		public async ValueTask<ConstraintResult> IsMetBy(IFileInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_fileContent = null;
			if (actual is null)
			{
				return this;
			}

			if (!actual.Exists)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			using StreamReader reader = actual.OpenText();
#if NET8_0_OR_GREATER
			_fileContent = await reader.ReadToEndAsync(cancellationToken);
#else
			cancellationToken.ThrowIfCancellationRequested();
			_fileContent = await reader.ReadToEndAsync();
			cancellationToken.ThrowIfCancellationRequested();
#endif
			_matchResult = await options.Value.MatchWithExplanation(_fileContent, expected, nameof(expected));
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_fileContent is not null)
			{
				contexts.Add(new ResultContext.Fixed(Constants.FileContentContext, _fileContent));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.IsPlural())
			{
				stringBuilder.Append("have content ");
			}
			else
			{
				stringBuilder.Append("has content ");
			}

			stringBuilder.Append(options.Value.GetExpectation(expected, Grammars));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!Actual!.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(options.Value.GetExtendedFailure(It,Grammars, _fileContent, expected, _matchResult));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have content ", "do not have content "))
				.Append(options.Value.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class HasBinaryContentConstraint(
		string it,
		ExpectationGrammars grammars,
		byte[] expected,
		string expectedExpression)
		: ConstraintResult.WithNotNullValue<IFileInfo>(it, grammars),
			IValueConstraint<IFileInfo>
	{
		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			if (!actual.Exists)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			byte[] content = actual.FileSystem.File.ReadAllBytes(actual.FullName);
			Outcome = content.SequenceEqual(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.IsPlural())
			{
				stringBuilder.Append("have content equal to ").Append(expectedExpression);
			}
			else
			{
				stringBuilder.Append("has content equal to ").Append(expectedExpression);
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!Actual!.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" differed");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have content equal to ", "do not have content equal to "))
				.Append(expectedExpression);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!Actual!.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" did match");
			}
		}
	}
}
