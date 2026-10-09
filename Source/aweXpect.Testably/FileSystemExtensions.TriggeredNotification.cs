using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class FileSystemExtensions
{
	/// <summary>
	///     Verifies that the <see cref="MockFileSystem.Notify" /> handler fired.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="INotificationHandler.OnEventOrReplay" /> so notifications
	///     that already fired on this <see cref="MockFileSystem" /> count toward the quantifier.
	///     The assertion always waits up to a timeout for late-arriving (asynchronous)
	///     notifications (30 seconds by default; use <c>.Within(timeout)</c> to override).
	/// </remarks>
	public static TriggeredNotificationResult TriggeredNotification(
		this IThat<MockFileSystem> subject)
		=> TriggeredNotificationCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockFileSystem.Notify" /> handler fired for a change
	///     matching the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="INotificationHandler.OnEventOrReplay" /> so notifications
	///     that already fired on this <see cref="MockFileSystem" /> count toward the quantifier.
	///     The assertion always waits up to a timeout for late-arriving (asynchronous)
	///     notifications (30 seconds by default; use <c>.Within(timeout)</c> to override).
	/// </remarks>
	public static TriggeredNotificationResult TriggeredNotification(
		this IThat<MockFileSystem> subject,
		Func<ChangeDescription, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return TriggeredNotificationCore(subject, predicate, doNotPopulateThisValue);
	}

	/// <summary>
	///     Verifies that the <see cref="MockFileSystem.Notify" /> handler did <i>not</i> fire.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="INotificationHandler.OnEventOrReplay" /> so any notification
	///     that already fired on this <see cref="MockFileSystem" /> fails the assertion. The
	///     assertion also waits up to a timeout for late-arriving notifications (30 seconds by
	///     default; use <c>.Within(timeout)</c> to lower it when you do not need to wait). The
	///     assertion short-circuits as soon as a matching notification is observed.
	/// </remarks>
	public static DidNotTriggerNotificationResult DidNotTriggerNotification(
		this IThat<MockFileSystem> subject)
		=> DidNotTriggerNotificationCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="MockFileSystem.Notify" /> handler did <i>not</i> fire for
	///     any change matching the <paramref name="predicate" />.
	/// </summary>
	public static DidNotTriggerNotificationResult DidNotTriggerNotification(
		this IThat<MockFileSystem> subject,
		Func<ChangeDescription, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		if (predicate is null)
		{
			throw new ArgumentNullException(nameof(predicate));
		}

		return DidNotTriggerNotificationCore(subject, predicate, doNotPopulateThisValue);
	}

	private static TriggeredNotificationResult TriggeredNotificationCore(
		IThat<MockFileSystem> subject,
		Func<ChangeDescription, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<ChangeDescription> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		return new TriggeredNotificationResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<MockFileSystem, ChangeDescription>(
					it, grammars,
					"triggered a notification",
					"did not trigger a notification",
					Subscribe,
					filter, quantifier, options)),
			subject,
			quantifier,
			options,
			filter);
	}

	private static DidNotTriggerNotificationResult DidNotTriggerNotificationCore(
		IThat<MockFileSystem> subject,
		Func<ChangeDescription, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<ChangeDescription> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		return new DidNotTriggerNotificationResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<MockFileSystem, ChangeDescription>(
					it, grammars,
					"triggered a notification",
					"did not trigger a notification",
					Subscribe,
					filter, quantifier, options).Invert()),
			subject,
			options,
			filter);
	}

	private static IAwaitableCallback<ChangeDescription> Subscribe(
		MockFileSystem fileSystem,
		Action<ChangeDescription> action)
		=> fileSystem.Notify.OnEventOrReplay(action);
}
