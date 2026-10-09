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
		=> new(
			expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasBinaryContentEqualToConstraint(
					it,
					grammars,
					expected,
					doNotPopulateThisValue)),
			subject);

	/// <summary>
	///     …is equal to the <paramref name="expected" /> string.
	/// </summary>
	public StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> EqualTo(
		string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasStringContentEqualToConstraint(
					it,
					grammars,
					options,
					expected)),
			subject, options);
	}

	/// <summary>
	///     …differs from the <paramref name="unexpected" /> binary.
	/// </summary>
	public AndOrResult<IFileInfo, IThat<IFileInfo>> NotEqualTo(
		byte[] unexpected,
		[CallerArgumentExpression("unexpected")]
		string doNotPopulateThisValue = "")
		=> new(
			expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasBinaryContentEqualToConstraint(
						it,
						grammars,
						unexpected,
						doNotPopulateThisValue)
					.Invert()),
			subject);

	/// <summary>
	///     …differs from the <paramref name="unexpected" /> string.
	/// </summary>
	public StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> NotEqualTo(
		string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasStringContentEqualToConstraint(
					it,
					grammars,
					options,
					unexpected).Invert()),
			subject, options);
	}

	/// <summary>
	///     …has the same content as the file on the <paramref name="filePath" />.
	/// </summary>
	public StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> SameAs(
		string filePath)
	{
		StringEqualityOptions options = new(nameof(filePath));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasContentSameAsConstraint(it, grammars, options, filePath)),
			subject, options);
	}

	/// <summary>
	///     …does not have the same content as the file on the <paramref name="filePath" />.
	/// </summary>
	public StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> NotSameAs(
		string filePath)
	{
		StringEqualityOptions options = new(nameof(filePath));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			expectationBuilder.And(" ").AddConstraint((it, grammars)
				=> new HasContentSameAsConstraint(it, grammars, options, filePath).Invert()),
			subject, options);
	}

	private sealed class HasBinaryContentEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		byte[] expected,
		string expectedExpression)
		: ConstraintResult.WithValue<IFileInfo>(it, grammars),
			IValueConstraint<IFileInfo>
	{
		/// <inheritdoc />
		public ConstraintResult IsMetBy(IFileInfo actual)
		{
			byte[] content = actual.FileSystem.File.ReadAllBytes(actual.FullName);
			Outcome = content.SequenceEqual(expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has content equal to ").Append(expectedExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" differed");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has content different from ").Append(expectedExpression);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did match");
	}

	private sealed class HasStringContentEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithValue<IFileInfo>(it, grammars),
			IAsyncConstraint<IFileInfo>
	{
		private string? _fileContent;

		/// <inheritdoc />
		public async ValueTask<ConstraintResult> IsMetBy(IFileInfo actual, CancellationToken cancellationToken)
		{
			_fileContent = actual.FileSystem.File.ReadAllText(actual.FullName);
			Outcome = await options.AreConsideredEqual(_fileContent, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.Add(new ResultContext.Fixed(FileContentContext, _fileContent));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has content ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _fileContent, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has content ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did match");
	}

	private sealed class HasContentSameAsConstraint(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expectedPath)
		: ConstraintResult.WithValue<IFileInfo>(it, grammars),
			IAsyncConstraint<IFileInfo>
	{
		private string? _expectedContent;
		private string? _fileContent;
		private string? _fullPath;
		private bool _isExpectedFound;

		/// <inheritdoc />
		public async ValueTask<ConstraintResult> IsMetBy(IFileInfo actual, CancellationToken cancellationToken)
		{
			_fileContent = actual.FileSystem.File.ReadAllText(actual.FullName);
			_fullPath = actual.FileSystem.Path.GetFullPath(expectedPath);
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
			=> contexts.Add(new ResultContext.Fixed(FileContentContext, _fileContent));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has the same content as file '").Append(_fullPath).Append('\'');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!_isExpectedFound)
			{
				stringBuilder.Append(It).Append(" did not contain any file at '").Append(_fullPath).Append('\'');
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _fileContent, _expectedContent));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have the same content as file '").Append(_fullPath).Append('\'');

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!_isExpectedFound)
			{
				stringBuilder.Append(It).Append(" did not contain any file at '").Append(_fullPath).Append('\'');
			}
			else
			{
				stringBuilder.Append(It).Append(" did match");
			}
		}
	}
}
