using System;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

/// <summary>
///     Extensions for <see cref="IFileSystemWatcher" />.
/// </summary>
public static class FileSystemWatcherExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileSystemWatcher" /> triggered an event.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="IWatcherTriggeredHandler.OnTriggeredOrReplay" />, so events
	///     that already fired on this watcher count toward the quantifier. The mock raises the
	///     events of a watcher asynchronously, so use <c>.Within(timeout)</c> to wait for them;
	///     without it, only the events raised so far count.
	///     The subject must be created from a <see cref="MockFileSystem" /> (calling this on a
	///     real-file-system watcher throws <see cref="InvalidOperationException" />). The watcher's
	///     <see cref="IFileSystemWatcher.EnableRaisingEvents" /> must be <see langword="true" /> for
	///     any event to be observed.
	/// </remarks>
	[GuaranteesNotNull]
	public static TriggeredWatcherResult Triggered(
		this IThat<IFileSystemWatcher> subject)
		=> TriggeredCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="IFileSystemWatcher" /> triggered an event for a change
	///     matching the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="IWatcherTriggeredHandler.OnTriggeredOrReplay" />, so events
	///     that already fired on this watcher count toward the quantifier. The mock raises the
	///     events of a watcher asynchronously, so use <c>.Within(timeout)</c> to wait for them;
	///     without it, only the events raised so far count.
	///     The subject must be created from a <see cref="MockFileSystem" /> (calling this on a
	///     real-file-system watcher throws <see cref="InvalidOperationException" />). The watcher's
	///     <see cref="IFileSystemWatcher.EnableRaisingEvents" /> must be <see langword="true" /> for
	///     any event to be observed.
	/// </remarks>
	[GuaranteesNotNull]
	public static TriggeredWatcherResult Triggered(
		this IThat<IFileSystemWatcher> subject,
		Func<WatcherChangeDescription, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));

		return TriggeredCore(subject, predicate, doNotPopulateThisValue);
	}

	/// <summary>
	///     Verifies that the <see cref="IFileSystemWatcher" /> did <i>not</i> trigger an event.
	/// </summary>
	/// <remarks>
	///     Subscribes via <see cref="IWatcherTriggeredHandler.OnTriggeredOrReplay" /> so any event
	///     that already fired on this watcher fails the assertion. Without <c>.Within(timeout)</c>,
	///     only the events raised so far count; with it, the assertion waits for the full timeout
	///     and fails as soon as a matching event is observed.
	///     The subject must be created from a <see cref="MockFileSystem" /> (calling this on a
	///     real-file-system watcher throws <see cref="InvalidOperationException" />). The watcher's
	///     <see cref="IFileSystemWatcher.EnableRaisingEvents" /> must be <see langword="true" /> for
	///     any event to be observed.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotTriggerWatcherResult DidNotTrigger(
		this IThat<IFileSystemWatcher> subject)
		=> DidNotTriggerCore(subject, null, "");

	/// <summary>
	///     Verifies that the <see cref="IFileSystemWatcher" /> did <i>not</i> trigger an event for
	///     any change matching the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static DidNotTriggerWatcherResult DidNotTrigger(
		this IThat<IFileSystemWatcher> subject,
		Func<WatcherChangeDescription, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));

		return DidNotTriggerCore(subject, predicate, doNotPopulateThisValue);
	}

	private static TriggeredWatcherResult TriggeredCore(
		IThat<IFileSystemWatcher> subject,
		Func<WatcherChangeDescription, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		RepeatedCheckOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<WatcherChangeDescription> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		return new TriggeredWatcherResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<IFileSystemWatcher, WatcherChangeDescription>(
					it, grammars,
					"an event",
					Subscribe,
					filter, quantifier, options)),
			subject,
			quantifier,
			options,
			filter);
	}

	private static DidNotTriggerWatcherResult DidNotTriggerCore(
		IThat<IFileSystemWatcher> subject,
		Func<WatcherChangeDescription, bool>? predicate,
		string predicateExpression)
	{
		Quantifier quantifier = new();
		RepeatedCheckOptions options = new();
		NotificationConstraints.TriggerNotificationFilter<WatcherChangeDescription> filter = new();
		if (predicate is not null)
		{
			filter.Add(predicate, predicateExpression);
		}

		return new DidNotTriggerWatcherResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.TriggeredNotificationConstraint<IFileSystemWatcher, WatcherChangeDescription>(
					it, grammars,
					"an event",
					Subscribe,
					filter, quantifier, options).Invert()),
			subject,
			options,
			filter);
	}

	private static IAwaitableCallback<WatcherChangeDescription> Subscribe(
		IFileSystemWatcher watcher,
		Action<WatcherChangeDescription> action)
	{
		if (watcher.FileSystem is not MockFileSystem mockFs)
		{
			throw new InvalidOperationException(
				"Triggered() and DidNotTrigger() require an IFileSystemWatcher created from a MockFileSystem.");
		}

		return mockFs.Watcher.OnTriggeredOrReplay(
			action,
			c => c.FileSystemWatcher == watcher);
	}
}
