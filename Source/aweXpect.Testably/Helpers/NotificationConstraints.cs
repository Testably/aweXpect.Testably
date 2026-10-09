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
		string normalExpectation,
		string negatedExpectation,
		Func<TSubject, Action<TChange>, IAwaitableCallback<TChange>> subscribe,
		TriggerNotificationFilter<TChange> filter,
		Quantifier quantifier,
		NotificationTimeoutOptions options)
		: ConstraintResult.WithValue<TSubject>(it, grammars),
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
				Outcome = Outcome.Failure;
				return this;
			}

			ConcurrentQueue<TChange> changes = new();
			// Not disposed, as a notification that is raised while the registration is disposed may still release it.
			SemaphoreSlim changeSignal = new(0);
			IAwaitableCallback<TChange> registration = subscribe(actual, change =>
			{
				changes.Enqueue(change);
				changeSignal.Release();
			});
			try
			{
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
					catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
					{
						break;
					}
				}
			}
			finally
			{
				registration.Dispose();
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
				cancellationToken.ThrowIfCancellationRequested();
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
				contexts.Visit(unanswered.Result);
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(normalExpectation);
			stringBuilder.Append(filter);
			stringBuilder.Append(' ').Append(quantifier);
			stringBuilder.Append(options);
			filter.AppendReasons(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder, indentation);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(negatedExpectation);
			stringBuilder.Append(filter);
			stringBuilder.Append(options);
			filter.AppendReasons(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCount(stringBuilder, indentation);

		private void AppendCount(StringBuilder stringBuilder, string? indentation)
		{
			if (Actual is null)
			{
				stringBuilder.Append(It).Append(" was <null>");
				return;
			}

			if (_unanswered is { } unanswered)
			{
				stringBuilder.Append("for change ").Append(unanswered.Change).Append(", ");
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
			stringBuilder.Append(" in [");
			for (int i = 0; i < _matches.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}

				stringBuilder.Append(Environment.NewLine).Append("  ").Append(_matches[i]);
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
