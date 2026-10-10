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

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for additional verifications on the content of a <see cref="IFileInfo" />.
/// </summary>
public class FileInfoContentResult(
	ExpectationBuilder expectationBuilder,
	IThat<IFileInfo> subject)
{
	private const string FileContentContext = "File content";

	/// <summary>
	///     …is equal to the <paramref name="expected" /> binary.
	/// </summary>
	public AndOrResult<IFileInfo, IThat<IFileInfo>> EqualTo(
		byte[] expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		return new AndOrResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.AddConstraint((expected, doNotPopulateThisValue), static (s, it, grammars)
				=> new HasBinaryContentEqualToConstraint(
					it,
					grammars,
					s.expected,
					s.doNotPopulateThisValue)),
			subject);
	}

	/// <summary>
	///     …is equal to the <paramref name="expected" /> string.
	/// </summary>
	public StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> EqualTo(
		string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new HasStringContentEqualToConstraint(
					it,
					grammars,
					s.options,
					s.expected)),
			subject, options);
	}

	/// <summary>
	///     …differs from the <paramref name="unexpected" /> binary.
	/// </summary>
	public AndOrResult<IFileInfo, IThat<IFileInfo>> NotEqualTo(
		byte[] unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));

		return new AndOrResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.AddConstraint((unexpected, doNotPopulateThisValue), static (s, it, grammars)
				=> new HasBinaryContentEqualToConstraint(
						it,
						grammars,
						s.unexpected,
						s.doNotPopulateThisValue)
					.Invert()),
			subject);
	}

	/// <summary>
	///     …differs from the <paramref name="unexpected" /> string.
	/// </summary>
	public StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> NotEqualTo(
		string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.AddConstraint((options, unexpected), static (s, it, grammars)
				=> new HasStringContentEqualToConstraint(
					it,
					grammars,
					s.options,
					s.unexpected).Invert()),
			subject, options);
	}

	/// <summary>
	///     …has the same content as the file on the <paramref name="filePath" />.
	/// </summary>
	public StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> SameAs(
		string filePath)
	{
		ThrowHelper.ThrowIfNull(filePath, nameof(filePath));

		StringEqualityOptions options = new(nameof(filePath));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.AddConstraint((options, filePath), static (s, it, grammars)
				=> new HasContentSameAsConstraint(it, grammars, s.options, s.filePath)),
			subject, options);
	}

	/// <summary>
	///     …does not have the same content as the file on the <paramref name="filePath" />.
	/// </summary>
	public StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> NotSameAs(
		string filePath)
	{
		ThrowHelper.ThrowIfNull(filePath, nameof(filePath));

		StringEqualityOptions options = new(nameof(filePath));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.AddConstraint((options, filePath), static (s, it, grammars)
				=> new HasContentSameAsConstraint(it, grammars, s.options, s.filePath).Invert()),
			subject, options);
	}

	private sealed class HasBinaryContentEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		byte[] expected,
		string expectedExpression)
		: ConstraintResult.WithNotNullValue<IFileInfo>(it, grammars),
			IValueConstraint<IFileInfo>
	{
		private bool _exists;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			_exists = actual.Exists;
			if (!_exists)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			byte[] content = actual.FileSystem.File.ReadAllBytes(actual.FullName);
			Outcome = content.SequenceEqual(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has content equal to ", "have content equal to "))
				.Append(expectedExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(_exists ? " differed" : " did not exist");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have content equal to ", "do not have content equal to "))
				.Append(expectedExpression);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(_exists ? " did match" : " did not exist");
	}

	private sealed class HasStringContentEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithNotNullValue<IFileInfo>(it, grammars),
			IAsyncConstraint<IFileInfo>
	{
		private string? _fileContent;

		/// <inheritdoc />
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

			_fileContent = actual.FileSystem.File.ReadAllText(actual.FullName);
			Outcome = await options.AreConsideredEqual(_fileContent, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_fileContent is not null)
			{
				contexts.Add(new ResultContext.Fixed(FileContentContext, _fileContent));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has content ", "have content "))
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_fileContent is null)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _fileContent, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have content ", "do not have content "))
				.Append(options.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(_fileContent is null ? " did not exist" : " did match");
	}

	private sealed class HasContentSameAsConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expectedPath)
		: ConstraintResult.WithNotNullValue<IFileInfo>(it, grammars),
			IAsyncConstraint<IFileInfo>
	{
		private string? _expectedContent;
		private string? _fileContent;
		private string? _fullPath;
		private bool _isExpectedFound;

		/// <inheritdoc />
		public async ValueTask<ConstraintResult> IsMetBy(IFileInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_fileContent = null;
			if (actual is null)
			{
				return this;
			}

			_fullPath = actual.FileSystem.Path.GetFullPath(expectedPath);
			if (!actual.Exists)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_fileContent = actual.FileSystem.File.ReadAllText(actual.FullName);
			_isExpectedFound = actual.FileSystem.File.Exists(expectedPath);
			if (!_isExpectedFound)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_expectedContent = actual.FileSystem.File.ReadAllText(expectedPath);
			Outcome = await options.AreConsideredEqual(_fileContent, _expectedContent) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_fileContent is not null)
			{
				contexts.Add(new ResultContext.Fixed(FileContentContext, _fileContent));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has the same content as file ", "have the same content as file "));
			Formatter.Format(stringBuilder, _fullPath ?? expectedPath);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_fileContent is null)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else if (!_isExpectedFound)
			{
				stringBuilder.Append(It).Append(" did not contain any file at ");
				Formatter.Format(stringBuilder, _fullPath);
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _fileContent, _expectedContent));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have the same content as file ", "do not have the same content as file "));
			Formatter.Format(stringBuilder, _fullPath ?? expectedPath);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_fileContent is null)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else if (!_isExpectedFound)
			{
				stringBuilder.Append(It).Append(" did not contain any file at ");
				Formatter.Format(stringBuilder, _fullPath);
			}
			else
			{
				stringBuilder.Append(It).Append(" did match");
			}
		}
	}
}
