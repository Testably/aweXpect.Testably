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
public partial class FileResult<TParent>
{
	/// <summary>
	///     The result for additional verifications on a file content.
	/// </summary>
	public class Content
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly Func<TParent, (IFileSystem fs, string fullPath)> _resolver;
		private readonly FileResult<TParent> _subject;

		internal Content(
			ExpectationBuilder expectationBuilder,
			FileResult<TParent> subject,
			Func<TParent, (IFileSystem fs, string fullPath)> resolver)
		{
			_expectationBuilder = expectationBuilder;
			_subject = subject;
			_resolver = resolver;
		}

		/// <summary>
		///     …is equal to the <paramref name="expected" /> binary.
		/// </summary>
		public AndOrResult<TParent, FileResult<TParent>> EqualTo(
			byte[] expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
			=> new(
				_expectationBuilder.And(" ").AddConstraint((it, grammars)
					=> new HasBinaryContentEqualToConstraint(
						it,
						grammars,
						_resolver,
						expected,
						doNotPopulateThisValue)),
				_subject);

		/// <summary>
		///     …is equal to the <paramref name="expected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TParent, FileResult<TParent>> EqualTo(
			string expected)
		{
			StringEqualityOptions options = new(nameof(expected));
			return new StringEqualityTypeResult<TParent, FileResult<TParent>>(
				_expectationBuilder.And(" ").AddConstraint((it, grammars)
					=> new HasStringContentEqualToConstraint(
						it,
						grammars,
						_resolver,
						options,
						expected)),
				_subject, options);
		}

		/// <summary>
		///     …differs from the <paramref name="unexpected" /> binary.
		/// </summary>
		public AndOrResult<TParent, FileResult<TParent>> NotEqualTo(
			byte[] unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
			=> new(
				_expectationBuilder.And(" ").AddConstraint((it, grammars)
					=> new HasBinaryContentEqualToConstraint(
							it,
							grammars,
							_resolver,
							unexpected,
							doNotPopulateThisValue,
							isInverted: true)
						.Invert()),
				_subject);

		/// <summary>
		///     …differs from the <paramref name="unexpected" /> string.
		/// </summary>
		public StringEqualityTypeResult<TParent, FileResult<TParent>> NotEqualTo(
			string unexpected)
		{
			StringEqualityOptions options = new(nameof(unexpected));
			return new StringEqualityTypeResult<TParent, FileResult<TParent>>(
				_expectationBuilder.And(" ").AddConstraint((it, grammars)
					=> new HasStringContentEqualToConstraint(
						it,
						grammars,
						_resolver,
						options,
						unexpected,
						isInverted: true).Invert()),
				_subject, options);
		}

		/// <summary>
		///     …has the same content as the file on the <paramref name="filePath" />.
		/// </summary>
		public StringEqualityTypeResult<TParent, FileResult<TParent>> SameAs(
			string filePath)
		{
			StringEqualityOptions options = new(nameof(filePath));
			return new StringEqualityTypeResult<TParent, FileResult<TParent>>(
				_expectationBuilder.And(" ").AddConstraint((it, grammars)
					=> new HasContentSameAsConstraint(it, grammars, _resolver, options, filePath)),
				_subject, options);
		}

		/// <summary>
		///     …does not have the same content as the file on the <paramref name="filePath" />.
		/// </summary>
		public StringEqualityTypeResult<TParent, FileResult<TParent>> NotSameAs(
			string filePath)
		{
			StringEqualityOptions options = new(nameof(filePath));
			return new StringEqualityTypeResult<TParent, FileResult<TParent>>(
				_expectationBuilder.And(" ").AddConstraint((it, grammars)
					=> new HasContentSameAsConstraint(it, grammars, _resolver, options, filePath, isInverted: true)
						.Invert()),
				_subject, options);
		}
	}

	private sealed class HasBinaryContentEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver,
		byte[] expected,
		string expectedExpression,
		bool isInverted = false)
		: ConstraintResult.WithNotNullValue<TParent>(it, grammars),
			IValueConstraint<TParent>
	{
		private string? _missingFileResult;

		public ConstraintResult IsMetBy(TParent actual)
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
		Func<TParent, (IFileSystem fs, string fullPath)> resolver,
		StringEqualityOptions options,
		string expected,
		bool isInverted = false)
		: ConstraintResult.WithNotNullValue<TParent>(it, grammars),
			IAsyncConstraint<TParent>
	{
		private string? _fileContent;
		private string? _missingFileResult;

		public async ValueTask<ConstraintResult> IsMetBy(TParent actual, CancellationToken cancellationToken)
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
			Outcome = await options.AreConsideredEqual(_fileContent, expected) ? Outcome.Success : Outcome.Failure;
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
			=> stringBuilder.Append("with content ").Append(options.GetExpectation(expected,
				isInverted ? Grammars | ExpectationGrammars.Negated : Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_missingFileResult is not null)
			{
				stringBuilder.Append(It).Append(_missingFileResult);
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _fileContent, expected));
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
		Func<TParent, (IFileSystem fs, string fullPath)> resolver,
		StringEqualityOptions options,
		string expectedPath,
		bool isInverted = false)
		: ConstraintResult.WithNotNullValue<TParent>(it, grammars),
			IAsyncConstraint<TParent>
	{
		private string? _expectedContent;
		private string? _fileContent;
		private string? _fullExpectedPath;
		private bool _isExpectedFound;
		private string? _missingFileResult;

		public async ValueTask<ConstraintResult> IsMetBy(TParent actual, CancellationToken cancellationToken)
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
			Outcome = await options.AreConsideredEqual(_fileContent, _expectedContent) ? Outcome.Success : Outcome.Failure;
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
			=> stringBuilder.Append(isInverted ? "with not the same content as file '" : "with the same content as file '")
				.Append(_fullExpectedPath ?? expectedPath).Append('\'');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_missingFileResult is not null)
			{
				stringBuilder.Append(It).Append(_missingFileResult);
			}
			else if (!_isExpectedFound)
			{
				stringBuilder.Append(It).Append(" did not contain any file at '").Append(_fullExpectedPath).Append('\'');
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _fileContent, _expectedContent));
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
				stringBuilder.Append(It).Append(" did not contain any file at '").Append(_fullExpectedPath).Append('\'');
			}
			else
			{
				stringBuilder.Append(It).Append(" did match");
			}
		}
	}

	/// <remarks>
	///     Repeats the result of <see cref="FileSystemConstraints.HasFileConstraint{TParent}" />, so that a combined
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
