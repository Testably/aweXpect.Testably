using System;
using System.Collections.Concurrent;
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
	/// <remarks>
	///     The notification callback only enqueues the changes, as it runs on the thread that raised the notification:
	///     the filters (code of the caller) are applied on the evaluation flow, so that they can neither block nor throw
	///     into the code under test.
	/// </remarks>
	internal sealed class TriggeredNotificationConstraint<TSubject, TChange>(
		string it,
		ExpectationGrammars grammars,
		string changeDescription,
		Func<TSubject, Action<TChange>, IAwaitableCallback<TChange>> subscribe,
		TriggerNotificationFilter<TChange> filter,
		Quantifier quantifier,
		RepeatedCheckOptions options)
		: ConstraintResult.WithNotNullValue<TSubject>(it, grammars),
			IAsyncContextConstraint<TSubject>,
			IExpectationTextConstraint
		where TSubject : class
		where TChange : ChangeDescription
	{
		private readonly List<TChange> _matches = new();
		private (TChange Change, ConstraintResult Result)? _unanswered;

		public async ValueTask<ConstraintResult> GetExpectationResult(IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			await filter.PrepareExpectation(context, cancellationToken);
			return this;
		}

		public async ValueTask<ConstraintResult> IsMetBy(TSubject actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_matches.Clear();
			_unanswered = null;
			await filter.PrepareExpectation(context, cancellationToken);
			if (actual is null)
			{
				return this;
			}

			ConcurrentQueue<TChange> changes = new();
			// Not disposed, as a notification that is raised while the registration is disposed may still release it.
			SemaphoreSlim changeSignal = new(0);
			long startTimestamp = context.GetTimestamp();
			IAwaitableCallback<TChange> registration = subscribe(actual, change =>
			{
				changes.Enqueue(change);
				changeSignal.Release();
			});
			try
			{
				// Without Within, the timeout is zero, so only the replayed changes are checked.
				using CancellationTokenSource deadline =
					CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				deadline.CancelAfter(options.Timeout);
				while (true)
				{
					if (await IsDetermined(changes, context, cancellationToken))
					{
						return this;
					}

					try
					{
						await changeSignal.WaitAsync(deadline.Token);
					}
					catch (OperationCanceledException)
					{
						break;
					}
				}
			}
			finally
			{
				registration.Dispose();
			}

			if (cancellationToken.IsCancellationRequested &&
			    !context.Cancellation.HasWaitElapsed(options.Timeout, context.GetElapsedTime(startTimestamp)))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			if (!await IsDetermined(changes, context, cancellationToken))
			{
				Outcome = quantifier.Check(_matches.Count, true) == true ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		/// <summary>
		///     Applies the filters to the received <paramref name="changes" /> and returns <see langword="true" /> when the
		///     outcome is decided without waiting for further changes.
		/// </summary>
		private async ValueTask<bool> IsDetermined(ConcurrentQueue<TChange> changes,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			while (changes.TryDequeue(out TChange? change))
			{
				if (!filter.IsMatch(change))
				{
					continue;
				}

				ConstraintResult? unmetResult = await filter.GetUnmetResult(change, context, cancellationToken);
				if (unmetResult?.Outcome == Outcome.Undecided)
				{
					cancellationToken.ThrowIfCancellationRequested();
				}
				if (unmetResult?.Outcome == Outcome.FailureBothWays)
				{
					_unanswered = (change, unmetResult);
					Outcome = Outcome.FailureBothWays;
					return true;
				}

				if (unmetResult is not null)
				{
					continue;
				}

				_matches.Add(change);
				if (quantifier.Check(_matches.Count, false) == true)
				{
					Outcome = Outcome.Success;
					return true;
				}
			}

			if (quantifier.Check(_matches.Count, false) == false)
			{
				Outcome = Outcome.Failure;
				return true;
			}

			return false;
		}

		public override Exception? FailureCause => _unanswered?.Result.FailureCause;

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_unanswered is { } unanswered)
			{
				contexts.Add(new ResultContext.Fixed("Change", Formatter.Format(unanswered.Change)));
				contexts.Visit(unanswered.Result);
			}
			else if (_matches.Count > 0)
			{
				TChange[] matches = _matches.ToArray();
				contexts.Add(new ResultContext.SyncCallback("Matching changes",
					() => Formatter.Format(matches, FormattingOptions.MultipleLines)));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, false);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder, indentation);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, true);

		private void AppendExpectation(StringBuilder stringBuilder, bool isNegated)
		{
			if (quantifier.IsNever(isNegated))
			{
				stringBuilder.Append("has never triggered ").Append(changeDescription).Append(filter);
			}
			else
			{
				stringBuilder.Append("has triggered ").Append(changeDescription).Append(filter)
					.Append(' ').Append(quantifier.ToString(isNegated));
			}

			stringBuilder.Append(options);
			filter.AppendReasons(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder, indentation);

		private void AppendCount(StringBuilder stringBuilder, string? indentation)
		{
			if (_unanswered is { } unanswered)
			{
				stringBuilder.Append(It).Append(" triggered ").Append(changeDescription).Append(" for which ");
				unanswered.Result.AppendResult(stringBuilder, indentation);
				return;
			}

			stringBuilder.Append(It).Append(" was ");
			if (_matches.Count == 0)
			{
				stringBuilder.Append("not triggered");
				return;
			}

			stringBuilder.Append("triggered ");
			AppendTimes(stringBuilder, _matches.Count);
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
			=> _syncPredicates.Add((predicate, predicateExpression.Trim()));

		public void Add(ManualExpectationBuilder<TChange> builder)
			=> _asyncFilters.Add(builder);

		/// <summary>
		///     Prepares the text of the nested expectations, so that it is complete also when no change is evaluated.
		/// </summary>
		public async Task PrepareExpectation(IEvaluationContext context, CancellationToken cancellationToken)
		{
			foreach (ManualExpectationBuilder<TChange> builder in _asyncFilters)
			{
				await builder.PrepareExpectation(context, cancellationToken);
			}
		}

		public void AppendReasons(StringBuilder stringBuilder)
		{
			foreach (ManualExpectationBuilder<TChange> builder in _asyncFilters)
			{
				builder.AppendReasons(stringBuilder);
			}
		}

		public bool IsMatch(TChange change)
			=> _syncPredicates.All(p => UserCode.Invoke(p.Predicate, change, "the predicate"));

		/// <summary>
		///     Returns the result of the first nested expectation that the <paramref name="change" /> does not meet, or
		///     <see langword="null" /> when it meets all of them.
		/// </summary>
		public async ValueTask<ConstraintResult?> GetUnmetResult(TChange change,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			foreach (ManualExpectationBuilder<TChange> builder in _asyncFilters)
			{
				ConstraintResult result = await builder.IsMetBy(change, context, cancellationToken);
				if (result.Outcome != Outcome.Success)
				{
					return result;
				}
			}

			return null;
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
				sb.Append(firstInGroup ? " that " : " and ");
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
		: ConstraintResult.WithNotNullValue<TChange>(it, grammars),
			IValueConstraint<TChange>
		where TChange : ChangeDescription
	{
		public ConstraintResult IsMetBy(TChange actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			Outcome = (actual.ChangeType & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has change type ", "have change type "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual!.ChangeType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have change type ", "do not have change type "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}

	internal sealed class HasFileSystemTypeConstraint<TChange>(
		string it,
		ExpectationGrammars grammars,
		FileSystemTypes expected)
		: ConstraintResult.WithNotNullValue<TChange>(it, grammars),
			IValueConstraint<TChange>
		where TChange : ChangeDescription
	{
		public ConstraintResult IsMetBy(TChange actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			Outcome = (actual.FileSystemType & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has file system type ", "have file system type "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual!.FileSystemType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have file system type ", "do not have file system type "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}

	internal sealed class HasNotifyFiltersConstraint<TChange>(
		string it,
		ExpectationGrammars grammars,
		NotifyFilters expected)
		: ConstraintResult.WithNotNullValue<TChange>(it, grammars),
			IValueConstraint<TChange>
		where TChange : ChangeDescription
	{
		public ConstraintResult IsMetBy(TChange actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			Outcome = (actual.NotifyFilters & expected) == expected
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has notify filters ", "have notify filters "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual!.NotifyFilters);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have notify filters ", "do not have notify filters "));
			Formatter.Format(stringBuilder, expected);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}

	internal sealed class HasStringPropertyConstraint<TChange>(
		string it,
		ExpectationGrammars grammars,
		Func<TChange, string?> selector,
		StringEqualityOptions options,
		string? expected,
		string propertyName)
		: ConstraintResult.WithNotNullValue<TChange>(it, grammars),
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
				return this;
			}

			_actualValue = selector(actual);
			Outcome = await options.AreConsideredEqual(_actualValue, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has ", "have ")).Append(propertyName).Append(' ')
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It,Grammars, _actualValue, expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have ", "do not have ")).Append(propertyName).Append(' ')
				.Append(options.GetExpectation(expected, Grammars & ~ExpectationGrammars.Negated));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
