using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Helpers;

internal static class StatisticsConstraints
{
	internal sealed class RecordedMethodCallConstraint(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier,
		Func<IFileSystemStatistics, IStatistics> bucketSelector,
		string bucketDescription,
		string methodName,
		ParameterMatcher[] matchers)
		: ConstraintResult.WithNotNullValue<IFileSystemStatistics>(it, grammars),
			IValueConstraint<IFileSystemStatistics>
	{
		private int _matchCount;

		public ConstraintResult IsMetBy(IFileSystemStatistics actual)
		{
			Actual = actual;
			_matchCount = 0;
			if (actual is null)
			{
				return this;
			}

			foreach (MethodStatistic method in bucketSelector(actual).Methods)
			{
				if (method.Name != methodName)
				{
					continue;
				}

				if (!MatchersSatisfied(method))
				{
					continue;
				}

				_matchCount++;
			}

			Outcome = quantifier.Check(_matchCount, true) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		private bool MatchersSatisfied(MethodStatistic method)
		{
			for (int i = 0; i < matchers.Length; i++)
			{
				if (matchers[i].IsAny)
				{
					continue;
				}

				if (i >= method.Parameters.Length)
				{
					return false;
				}

				if (!matchers[i].IsMatch(method.Parameters[i]))
				{
					return false;
				}
			}

			return true;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, false);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, true);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendResult(stringBuilder);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendResult(stringBuilder);

		private void AppendExpectation(StringBuilder stringBuilder, bool negated)
		{
			if (quantifier.IsNever(false))
			{
				stringBuilder.Append(negated ? "recorded at least one call to " : "recorded no call to ")
					.Append(bucketDescription).Append('.').Append(methodName);
				AppendMatcherDescription(stringBuilder);
			}
			else
			{
				stringBuilder.Append(negated ? "did not record a call to " : "recorded a call to ")
					.Append(bucketDescription).Append('.').Append(methodName);
				AppendMatcherDescription(stringBuilder);
				stringBuilder.Append(' ').Append(quantifier);
			}
		}

		private void AppendMatcherDescription(StringBuilder stringBuilder)
		{
			bool any = false;
			for (int i = 0; i < matchers.Length; i++)
			{
				if (matchers[i].IsAny)
				{
					continue;
				}

				stringBuilder.Append(any ? ", " : " with ");
				stringBuilder.Append(matchers[i].Name);
				if (matchers[i].Expression is { } expression)
				{
					stringBuilder.Append(" matching ").Append(expression);
				}

				any = true;
			}
		}

		private void AppendResult(StringBuilder stringBuilder)
		{
			stringBuilder.Append(It).Append(" was recorded ").Append(_matchCount).Append(" time");
			if (_matchCount != 1)
			{
				stringBuilder.Append('s');
			}
		}
	}

	internal sealed class RecordedPropertyAccessConstraint(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier,
		Func<IFileSystemStatistics, IStatistics> bucketSelector,
		string bucketDescription,
		string propertyName,
		PropertyAccess access)
		: ConstraintResult.WithNotNullValue<IFileSystemStatistics>(it, grammars),
			IValueConstraint<IFileSystemStatistics>
	{
		private int _matchCount;

		public ConstraintResult IsMetBy(IFileSystemStatistics actual)
		{
			Actual = actual;
			_matchCount = 0;
			if (actual is null)
			{
				return this;
			}

			foreach (PropertyStatistic property in bucketSelector(actual).Properties)
			{
				if (property.Name == propertyName && property.Access == access)
				{
					_matchCount++;
				}
			}

			Outcome = quantifier.Check(_matchCount, true) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, false);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, true);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendResult(stringBuilder);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendResult(stringBuilder);

		private void AppendExpectation(StringBuilder stringBuilder, bool negated)
		{
			string accessVerb = access == PropertyAccess.Get ? "get of " : "set of ";
			if (quantifier.IsNever(false))
			{
				stringBuilder.Append(negated ? "recorded at least one " : "recorded no ").Append(accessVerb)
					.Append(bucketDescription).Append('.').Append(propertyName);
			}
			else
			{
				stringBuilder.Append(negated ? "did not record a " : "recorded a ").Append(accessVerb)
					.Append(bucketDescription).Append('.').Append(propertyName)
					.Append(' ').Append(quantifier);
			}
		}

		private void AppendResult(StringBuilder stringBuilder)
		{
			stringBuilder.Append(It).Append(" was recorded ").Append(_matchCount).Append(" time");
			if (_matchCount != 1)
			{
				stringBuilder.Append('s');
			}
		}
	}
}
