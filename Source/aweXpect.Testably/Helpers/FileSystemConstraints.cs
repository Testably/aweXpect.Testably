using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect.Testably.Helpers;

internal static class FileSystemConstraints
{
	internal static string? GetMissingResult(IFileSystemInfo info)
		=> info.Exists ? null : " did not exist";

	internal sealed class HasAttributeConstraint<TInfo>(
		string it,
		ExpectationGrammars grammars,
		FileAttributes expected)
		: ConstraintResult.WithValue<TInfo>(it, grammars),
			IValueConstraint<TInfo>
		where TInfo : class, IFileSystemInfo
	{
		private FileAttributes _actualAttributes;

		public ConstraintResult IsMetBy(TInfo actual)
		{
			Actual = actual;
			if (actual is null || !actual.Exists)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualAttributes = actual.Attributes;
			Outcome = (_actualAttributes & expected) == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has attribute ", "have attribute "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else if (!Actual.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ");
				Formatter.Format(stringBuilder, _actualAttributes);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have attribute ", "do not have attribute "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else if (!Actual.Exists)
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}
	}

	internal sealed class ExistsConstraint<TInfo>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<TInfo>(it, grammars),
			IValueConstraint<TInfo>
		where TInfo : class, IFileSystemInfo
	{
		public ConstraintResult IsMetBy(TInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = actual.Exists ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("exists", "exist"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Actual is null ? " was <null>" : " did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not exist", "do not exist"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Actual is null ? " was <null>" : " did");
	}

	internal sealed class HasNameConstraint<TInfo>(
		string it,
		ExpectationGrammars grammars,
		StringEqualityOptions options,
		string expected)
		: ConstraintResult.WithValue<TInfo>(it, grammars),
			IAsyncConstraint<TInfo>
		where TInfo : class, IFileSystemInfo
	{
		private string? _actualName;

		public async ValueTask<ConstraintResult> IsMetBy(TInfo actual, CancellationToken cancellationToken)
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
			=> stringBuilder.Append(Grammars.Verb("has name ", "have name ")).Append(options.GetExpectation(expected, Grammars));

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
			=> stringBuilder.Append(Grammars.Verb("does not have name ", "do not have name "))
				.Append(options.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Actual is null ? " was <null>" : " did");
	}

	internal sealed class HasFileConstraint<TParent>(
		string it,
		ExpectationGrammars grammars,
		string path,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver)
		: ConstraintResult.WithValue<TParent>(it, grammars),
			IValueConstraint<TParent>
		where TParent : class
	{
		private IFileSystem? _fs;
		private string? _fullPath;

		public ConstraintResult IsMetBy(TParent actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			(_fs, _fullPath) = resolver(actual);
			Outcome = _fs.File.Exists(_fullPath) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has file ", "have file "));
			Formatter.Format(stringBuilder, path);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else if (_fs?.Directory.Exists(_fullPath) == true)
			{
				stringBuilder.Append(It).Append(" was a directory");
			}
			else
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have file ", "do not have file "));
			Formatter.Format(stringBuilder, path);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Actual is null ? " was <null>" : " did");
	}

	internal sealed class HasDirectoryConstraint<TParent>(
		string it,
		ExpectationGrammars grammars,
		string path,
		Func<TParent, (IFileSystem fs, string fullPath)> resolver)
		: ConstraintResult.WithValue<TParent>(it, grammars),
			IValueConstraint<TParent>
		where TParent : class
	{
		private IFileSystem? _fs;
		private string? _fullPath;

		public ConstraintResult IsMetBy(TParent actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			(_fs, _fullPath) = resolver(actual);
			Outcome = _fs.Directory.Exists(_fullPath) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has directory ", "have directory "));
			Formatter.Format(stringBuilder, path);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else if (_fs?.File.Exists(_fullPath) == true)
			{
				stringBuilder.Append(It).Append(" was a file");
			}
			else
			{
				stringBuilder.Append(It).Append(" did not exist");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have directory ", "do not have directory "));
			Formatter.Format(stringBuilder, path);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Actual is null ? " was <null>" : " did");
	}

	internal sealed class HasTimeConstraint<TActual>(
		string it,
		ExpectationGrammars grammars,
		Func<TActual, DateTime> timeAccessor,
		Func<TActual, string?> getMissingResult,
		TimeTolerance tolerance,
		DateTime expected,
		string expectedString,
		bool isWithClause)
		: ConstraintResult.WithValue<TActual>(it, grammars),
			IValueConstraint<TActual>
		where TActual : class
	{
		private DateTime _actualTime;
		private string? _missingResult;

		public ConstraintResult IsMetBy(TActual actual)
		{
			Actual = actual;
			_missingResult = null;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_missingResult = getMissingResult(actual);
			if (_missingResult is not null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualTime = timeAccessor(actual);
			if (expected.Kind == DateTimeKind.Utc && _actualTime.Kind == DateTimeKind.Local)
			{
				_actualTime = _actualTime.ToUniversalTime();
			}

			if (expected.Kind == DateTimeKind.Local && _actualTime.Kind == DateTimeKind.Utc)
			{
				_actualTime = _actualTime.ToLocalTime();
			}

			TimeSpan timeTolerance = tolerance.GetToleranceOrDefault();
			TimeSpan difference = _actualTime - expected;
			Outcome = difference <= timeTolerance && difference >= timeTolerance.Negate()
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(isWithClause ? "with" : Grammars.Verb("has", "have"))
				.Append(' ').Append(expectedString).Append(" equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else if (_missingResult is not null)
			{
				stringBuilder.Append(It).Append(_missingResult);
			}
			else
			{
				stringBuilder.Append(It).Append(" was ");
				Formatter.Format(stringBuilder, _actualTime);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(isWithClause ? "with" : Grammars.Verb("does not have", "do not have"))
				.Append(' ').Append(expectedString).Append(" equal to ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(tolerance);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
