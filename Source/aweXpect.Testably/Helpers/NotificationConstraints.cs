using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Helpers;

internal static class NotificationConstraints
{
	internal sealed class TriggeredNotificationConstraint<TSubject, TChange>(
		string it,
		ExpectationGrammars grammars,
		string normalExpectation,
		string negatedExpectation,
		Func<TSubject, Action<TChange>, Func<TChange, bool>, IAwaitableCallback<TChange>> subscribe,
		TriggerNotificationFilter<TChange> filter,
		Quantifier quantifier,
		NotificationTimeoutOptions options,
		List<TChange> matches,
		bool exitOnFirstMatch = false)
		: ConstraintResult.WithValue<TSubject>(it, grammars),
			IAsyncContextConstraint<TSubject>
		where TSubject : class
		where TChange : ChangeDescription
	{
		public async ValueTask<ConstraintResult> IsMetBy(TSubject actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			TimeSpan timeout = options.Timeout;
			TaskCompletionSource<bool> earlyExit = new();

			using CancellationTokenSource deadlineCts =
				CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			deadlineCts.CancelAfter(timeout);
			CancellationToken deadlineToken = deadlineCts.Token;

			using IAwaitableCallback<TChange> registration = subscribe(
				actual,
				change =>
				{
					if (deadlineToken.IsCancellationRequested ||
					    !filter.IsAsyncMatchSync(change, context, deadlineToken))
					{
						return;
					}

					lock (matches)
					{
						if (exitOnFirstMatch && matches.Count > 0)
						{
							return;
						}

						matches.Add(change);
						if (exitOnFirstMatch || quantifier.Check(matches.Count, false) is not null)
						{
							earlyExit.TrySetResult(false);
						}
					}
				},
				filter.IsSyncMatch);

			if (!deadlineToken.IsCancellationRequested && !earlyExit.Task.IsCompleted)
			{
				await Task.WhenAny(
					earlyExit.Task,
					Task.Delay(Timeout.InfiniteTimeSpan, deadlineToken)).ConfigureAwait(false);
			}

			cancellationToken.ThrowIfCancellationRequested();

			int matchCount;
			lock (matches)
			{
				matchCount = matches.Count;
			}

			Outcome = quantifier.Check(matchCount, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(normalExpectation);
			stringBuilder.Append(filter);
			stringBuilder.Append(' ').Append(quantifier);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(negatedExpectation);
			stringBuilder.Append(filter);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder);

		private void AppendCount(StringBuilder stringBuilder)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
				return;
			}

			TChange[] snapshot;
			lock (matches)
			{
				snapshot = matches.ToArray();
			}

			stringBuilder.Append(It).Append(" was ");
			if (snapshot.Length == 0)
			{
				stringBuilder.Append("not triggered");
				return;
			}

			stringBuilder.Append("triggered ");
			AppendTimes(stringBuilder, snapshot.Length);
			stringBuilder.Append(" in [");
			for (int i = 0; i < snapshot.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}

				stringBuilder.Append(Environment.NewLine).Append("  ").Append(snapshot[i]);
			}

			stringBuilder.Append(Environment.NewLine).Append(']');
		}

		private static void AppendTimes(StringBuilder stringBuilder, int count)
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

	internal sealed class TriggerNotificationFilter<TChange>
		where TChange : ChangeDescription
	{
		private readonly List<ManualExpectationBuilder<TChange>> _asyncFilters = new();
		private readonly List<(Func<TChange, bool> Predicate, string Description)> _syncPredicates = new();

		public void Add(Func<TChange, bool> predicate, string predicateExpression)
		{
			if (predicate is null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			_syncPredicates.Add((predicate, predicateExpression.Trim()));
		}

		public void Add(ManualExpectationBuilder<TChange> builder)
			=> _asyncFilters.Add(builder);

		public bool IsSyncMatch(TChange change)
			=> _syncPredicates.All(p => p.Predicate(change));

		public bool IsAsyncMatchSync(TChange change,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			foreach (ManualExpectationBuilder<TChange> builder in _asyncFilters)
			{
				ConstraintResult result = builder.IsMetBy(change, context, cancellationToken)
					.AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
				if (result.Outcome != Outcome.Success)
				{
					return false;
				}
			}

			return true;
		}

		public override string ToString()
		{
			if (_syncPredicates.Count == 0 && _asyncFilters.Count == 0)
			{
				return "";
			}

			StringBuilder sb = new();
			bool firstInGroup = true;
			foreach ((Func<TChange, bool> _, string description) in _syncPredicates)
			{
				sb.Append(firstInGroup ? " matching " : " and ").Append(description);
				firstInGroup = false;
			}

			firstInGroup = true;
			foreach (ManualExpectationBuilder<TChange> builder in _asyncFilters)
			{
				sb.Append(firstInGroup ? " which " : " and ");
				builder.AppendExpectation(sb, "");
				firstInGroup = false;
			}

			return sb.ToString();
		}
	}

	internal sealed class HasChangeTypeConstraint<TChange>(
		string it,
		ExpectationGrammars grammars,
		WatcherChangeTypes expected)
		: ConstraintResult.WithValue<TChange>(it, grammars),
			IValueConstraint<TChange>
		where TChange : ChangeDescription
	{
		public ConstraintResult IsMetBy(TChange actual)
		{
			Actual = actual;
			Outcome = actual != null! && (actual.ChangeType & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has change type ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ").Append(Actual.ChangeType);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have change type ").Append(expected);

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

	internal sealed class HasFileSystemTypeConstraint<TChange>(
		string it,
		ExpectationGrammars grammars,
		FileSystemTypes expected)
		: ConstraintResult.WithValue<TChange>(it, grammars),
			IValueConstraint<TChange>
		where TChange : ChangeDescription
	{
		public ConstraintResult IsMetBy(TChange actual)
		{
			Actual = actual;
			Outcome = actual != null! && (actual.FileSystemType & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has file system type ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ").Append(Actual.FileSystemType);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have file system type ").Append(expected);

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

	internal sealed class HasNotifyFiltersConstraint<TChange>(
		string it,
		ExpectationGrammars grammars,
		NotifyFilters expected)
		: ConstraintResult.WithValue<TChange>(it, grammars),
			IValueConstraint<TChange>
		where TChange : ChangeDescription
	{
		public ConstraintResult IsMetBy(TChange actual)
		{
			Actual = actual;
			Outcome = actual != null! && (actual.NotifyFilters & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has notify filters ").Append(expected);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
			}
			else
			{
				stringBuilder.Append(It).Append(" was ").Append(Actual.NotifyFilters);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have notify filters ").Append(expected);

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

	internal sealed class HasStringPropertyConstraint<TChange>(
		string it,
		ExpectationGrammars grammars,
		Func<TChange, string?> selector,
		StringEqualityOptions options,
		string? expected,
		string propertyName)
		: ConstraintResult.WithValue<TChange>(it, grammars),
			IAsyncConstraint<TChange>
		where TChange : ChangeDescription
	{
		private string? _actualValue;

		public async ValueTask<ConstraintResult> IsMetBy(TChange actual,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
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
				.Append(options.GetExpectation(expected, Grammars));

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
}
