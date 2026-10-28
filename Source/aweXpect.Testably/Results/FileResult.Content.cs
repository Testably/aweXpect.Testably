using System;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on a file.
/// </summary>
public partial class FileResult<TType, TThat>
{
	/// <summary>
	///     The result for additional verifications on a file content.
	/// </summary>
	public class Content
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly Func<TType, (IFileSystem fs, string fullPath)> _resolver;
		private readonly FileResult<TType, TThat> _subject;

		internal Content(
			ExpectationBuilder expectationBuilder,
			FileResult<TType, TThat> subject,
			Func<TType, (IFileSystem fs, string fullPath)> resolver)
		{
			_expectationBuilder = expectationBuilder;
			_subject = subject;
			_resolver = resolver;
		}

		/// <summary>
		///     …is equal to the <paramref name="expected" /> binary.
		/// </summary>
		public AndOrResult<TType, FileResult<TType, TThat>> EqualTo(
			byte[] expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		{
			ThrowHelper.ThrowIfNull(expected, nameof(expected));

			return new AndOrResult<TType, FileResult<TType, TThat>>(
				_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, expected, doNotPopulateThisValue), static (s, it, grammars)
					=> new HasBinaryContentEqualToConstraint(
						it,
						grammars,
						s.resolver,
						s.expected,
						s.doNotPopulateThisValue)),
				_subject);
		}

		/// <summary>
		///     …is equal to the <paramref name="expected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TType, FileResult<TType, TThat>> EqualTo(
			string expected)
		{
			OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
			return new StringEqualityTypeResult<TType, FileResult<TType, TThat>>(
				_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, options, expected), static (s, it, grammars)
					=> new HasStringContentEqualToConstraint(
						it,
						grammars,
						s.resolver,
						s.options,
						s.expected,
						nameof(expected))),
				_subject, options);
		}

		/// <summary>
		///     …differs from the <paramref name="unexpected" /> binary.
		/// </summary>
		public AndOrResult<TType, FileResult<TType, TThat>> NotEqualTo(
			byte[] unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
		{
			ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));

			return new AndOrResult<TType, FileResult<TType, TThat>>(
				_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, unexpected, doNotPopulateThisValue), static (s, it, grammars)
					=> new HasBinaryContentEqualToConstraint(
							it,
							grammars,
							s.resolver,
							s.unexpected,
							s.doNotPopulateThisValue,
							isInverted: true)
						.Invert()),
				_subject);
		}

		/// <summary>
		///     …differs from the <paramref name="unexpected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TType, FileResult<TType, TThat>> NotEqualTo(
			string unexpected)
		{
			OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
			return new StringEqualityTypeResult<TType, FileResult<TType, TThat>>(
				_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, options, unexpected), static (s, it, grammars)
					=> new HasStringContentEqualToConstraint(
						it,
						grammars,
						s.resolver,
						s.options,
						s.unexpected,
						nameof(unexpected),
						isInverted: true).Invert()),
				_subject, options);
		}

		/// <summary>
		///     …has the same content as the file on the <paramref name="filePath" />.
		/// </summary>
		public StringEqualityTypeResult<TType, FileResult<TType, TThat>> SameAs(
			string filePath)
		{
			ThrowHelper.ThrowIfNull(filePath, nameof(filePath));

			OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
			return new StringEqualityTypeResult<TType, FileResult<TType, TThat>>(
				_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, options, filePath), static (s, it, grammars)
					=> new HasContentSameAsConstraint(it, grammars, s.resolver, s.options, s.filePath, nameof(filePath))),
				_subject, options);
		}

		/// <summary>
		///     …does not have the same content as the file on the <paramref name="filePath" />.
		/// </summary>
		public StringEqualityTypeResult<TType, FileResult<TType, TThat>> NotSameAs(
			string filePath)
		{
			ThrowHelper.ThrowIfNull(filePath, nameof(filePath));

			OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
			return new StringEqualityTypeResult<TType, FileResult<TType, TThat>>(
				_expectationBuilder.And(" ").AddConstraint((resolver: _resolver, options, filePath), static (s, it, grammars)
					=> new HasContentSameAsConstraint(it, grammars, s.resolver, s.options, s.filePath, nameof(filePath), isInverted: true)
						.Invert()),
				_subject, options);
		}
	}

	private sealed class HasBinaryContentEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TType, (IFileSystem fs, string fullPath)> resolver,
		byte[] expected,
		string expectedExpression,
		bool isInverted = false)
		: ConstraintResult.WithNotNullValue<TType>(it, grammars),
			IValueConstraint<TType>
	{
		private string? _missingFileResult;

		public ConstraintResult IsMetBy(TType actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			(IFileSystem fs, string fullPath) = resolver(actual);
			_missingFileResult = GetMissingFileResult(fs, fullPath);
			if (_missingFileResult is not null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			byte[] content = fs.File.ReadAllBytes(fullPath);
			Outcome = content.SequenceEqual(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(isInverted ? "with content different from " : "with content equal to ")
				.Append(expectedExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(_missingFileResult ?? " differed");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(_missingFileResult ?? " did match");
	}

	private sealed class HasStringContentEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TType, (IFileSystem fs, string fullPath)> resolver,
		OptionsHolder<StringEqualityOptions> options,
		string expected,
		string parameterName,
		bool isInverted = false)
		: ConstraintResult.WithNotNullValue<TType>(it, grammars),
			IAsyncConstraint<TType>
	{
		private MatchResult _matchResult;
		private string? _fileContent;
		private string? _missingFileResult;

		public async ValueTask<ConstraintResult> IsMetBy(TType actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_fileContent = null;
			if (actual is null)
			{
				return this;
			}

			(IFileSystem fs, string fullPath) = resolver(actual);
			_missingFileResult = GetMissingFileResult(fs, fullPath);
			if (_missingFileResult is not null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_fileContent = fs.File.ReadAllText(fullPath);
			_matchResult = await options.Value.MatchWithExplanation(_fileContent, expected, parameterName);
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
			=> stringBuilder.Append("with content ").Append(options.Value.GetExpectation(expected,
				isInverted ? Grammars | ExpectationGrammars.Negated : Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_missingFileResult is not null)
			{
				stringBuilder.Append(It).Append(_missingFileResult);
			}
			else
			{
				stringBuilder.Append(options.Value.GetExtendedFailure(It,Grammars, _fileContent, expected, _matchResult));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(_missingFileResult ?? " did match");
	}

	private sealed class HasContentSameAsConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TType, (IFileSystem fs, string fullPath)> resolver,
		OptionsHolder<StringEqualityOptions> options,
		string expectedPath,
		string parameterName,
		bool isInverted = false)
		: ConstraintResult.WithNotNullValue<TType>(it, grammars),
			IAsyncConstraint<TType>
	{
		private MatchResult _matchResult;
		private string? _expectedContent;
		private string? _fileContent;
		private string? _fullExpectedPath;
		private bool _isExpectedFound;
		private string? _missingFileResult;

		public async ValueTask<ConstraintResult> IsMetBy(TType actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_fileContent = null;
			if (actual is null)
			{
				return this;
			}

			(IFileSystem fs, string fullPath) = resolver(actual);
			_fullExpectedPath = fs.Path.GetFullPath(expectedPath);
			_missingFileResult = GetMissingFileResult(fs, fullPath);
			if (_missingFileResult is not null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_fileContent = fs.File.ReadAllText(fullPath);
			_isExpectedFound = fs.File.Exists(expectedPath);
			if (!_isExpectedFound)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_expectedContent = fs.File.ReadAllText(expectedPath);
			_matchResult = await options.Value.MatchWithExplanation(_fileContent, _expectedContent, parameterName);
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
			stringBuilder.Append(isInverted ? "with not the same content as file " : "with the same content as file ");
			Formatter.Format(stringBuilder, _fullExpectedPath ?? expectedPath);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_missingFileResult is not null)
			{
				stringBuilder.Append(It).Append(_missingFileResult);
			}
			else if (!_isExpectedFound)
			{
				stringBuilder.Append(It).Append(" did not contain any file at ");
				Formatter.Format(stringBuilder, _fullExpectedPath);
			}
			else
			{
				stringBuilder.Append(options.Value.GetExtendedFailure(It,Grammars, _fileContent, _expectedContent, _matchResult));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_missingFileResult is not null)
			{
				stringBuilder.Append(It).Append(_missingFileResult);
			}
			else if (!_isExpectedFound)
			{
				stringBuilder.Append(It).Append(" did not contain any file at ");
				Formatter.Format(stringBuilder, _fullExpectedPath);
			}
			else
			{
				stringBuilder.Append(It).Append(" did match");
			}
		}
	}

	/// <remarks>
	///     Repeats the result of <see cref="FileSystemConstraints.HasFileConstraint{TType}" />, so that a combined
	///     failure reports a missing file only once.
	/// </remarks>
	private static string? GetMissingFileResult(IFileSystem fs, string fullPath)
	{
		if (fs.File.Exists(fullPath))
		{
			return null;
		}

		return fs.Directory.Exists(fullPath) ? " was a directory" : " did not exist";
	}
}
