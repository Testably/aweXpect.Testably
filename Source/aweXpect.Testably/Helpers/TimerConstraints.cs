using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;
using Testably.Abstractions.Testing.TimeSystem;

namespace aweXpect.Testably.Helpers;

internal static class TimerConstraints
{
	internal sealed class TimerExecutedConstraint(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier,
		RepeatedCheckOptions options)
		: ConstraintResult.WithNotNullValue<ITimerMock>(it, grammars),
			IAsyncContextConstraint<ITimerMock>
	{
		private long _executionCount;

		/// <remarks>
		///     The checks stop as soon as the quantifier is decided either way, as the execution count only grows;
		///     otherwise the count of the last check decides, e.g. for <c>Never()</c> at the timeout.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(ITimerMock actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			Outcome outcome = await options.CheckRepeatedly(_ =>
			{
				_executionCount = actual.ExecutionCount;
				return new ValueTask<bool>(quantifier.Check(ToInt(_executionCount), false) is not null);
			}, context);
			if (outcome == Outcome.Undecided)
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			Outcome = quantifier.Check(ToInt(_executionCount), true) == true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		private static int ToInt(long value)
			=> value > int.MaxValue ? int.MaxValue : (int)value;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, false);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, true);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder);

		private void AppendExpectation(StringBuilder stringBuilder, bool negated)
		{
			if (quantifier.IsNever(false))
			{
				stringBuilder.Append(negated ? "has executed at least once" : "has never executed");
			}
			else
			{
				stringBuilder.Append(negated ? "has not executed " : "has executed ").Append(quantifier);
			}

			stringBuilder.Append(options);
		}

		private void AppendCount(StringBuilder stringBuilder)
		{
			stringBuilder.Append(It).Append(" was ");
			if (_executionCount == 0)
			{
				stringBuilder.Append("not executed");
				return;
			}

			stringBuilder.Append("executed ");
			AppendTimes(stringBuilder, _executionCount);
		}

		private static void AppendTimes(StringBuilder stringBuilder, long count)
		{
			switch (count)
			{
				case 1:
					stringBuilder.Append("once");
					break;
				case 2:
					stringBuilder.Append("twice");
					break;
				default:
					stringBuilder.Append(count).Append(" times");
					break;
			}
		}
	}
}
