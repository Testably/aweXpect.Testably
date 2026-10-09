using System;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect.Testably.Helpers;

internal static class FileVersionInfoConstraints
{
	internal sealed class HasStringPropertyConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<IFileVersionInfo, string?> selector,
		StringEqualityOptions options,
		string? expected,
		string propertyName)
		: ConstraintResult.WithValue<IFileVersionInfo>(it, grammars),
			IAsyncConstraint<IFileVersionInfo>
	{
		private string? _actualValue;

		public async ValueTask<ConstraintResult> IsMetBy(IFileVersionInfo actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualValue = selector(actual);
			Outcome = await options.AreConsideredEqual(_actualValue, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has ").Append(propertyName).Append(' ')
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _actualValue, expected));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have ").Append(propertyName).Append(' ')
				.Append(options.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}
	}

	internal sealed class HasInt32PropertyConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<IFileVersionInfo, int> selector,
		int expected,
		string propertyName)
		: ConstraintResult.WithValue<IFileVersionInfo>(it, grammars),
			IValueConstraint<IFileVersionInfo>
	{
		private int _actualValue;

		public ConstraintResult IsMetBy(IFileVersionInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualValue = selector(actual);
			Outcome = _actualValue == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has ").Append(propertyName).Append(' ').Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ").Append(_actualValue);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have ").Append(propertyName).Append(' ').Append(expected);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
		}
	}

	internal sealed class HasBoolPropertyConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<IFileVersionInfo, bool> selector,
		string normalExpectation,
		string negatedExpectation)
		: ConstraintResult.WithValue<IFileVersionInfo>(it, grammars),
			IValueConstraint<IFileVersionInfo>
	{
		public ConstraintResult IsMetBy(IFileVersionInfo actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = selector(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(normalExpectation);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was not");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(negatedExpectation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was");
			}
		}
	}
}
