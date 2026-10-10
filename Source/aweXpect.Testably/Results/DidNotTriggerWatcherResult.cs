using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for <see cref="FileSystemWatcherExtensions.DidNotTrigger(aweXpect.Core.IThat{IFileSystemWatcher})" />.
/// </summary>
public class DidNotTriggerWatcherResult
	: AndOrResult<IFileSystemWatcher, IThat<IFileSystemWatcher>, DidNotTriggerWatcherResult>
{
	private readonly NotificationConstraints.TriggerNotificationFilter<WatcherChangeDescription> _filter;
	private readonly RepeatedCheckOptions _options;

	internal DidNotTriggerWatcherResult(
		ExpectationBuilder expectationBuilder,
		IThat<IFileSystemWatcher> subject,
		RepeatedCheckOptions options, NotificationConstraints.TriggerNotificationFilter<WatcherChangeDescription> filter)
		: base(expectationBuilder, subject)
	{
		_options = options;
		_filter = filter;
	}

	/// <summary>
	///     Restricts the assertion to events that satisfy the inner <paramref name="expectation" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectation" /> is applied as an additional per-event filter, so any
	///     assertions from <see cref="ChangeDescriptionExtensions" /> (e.g. <c>.HasName(...)</c>,
	///     <c>.HasChangeType(...)</c>) compose naturally. The assertion fails if any event
	///     satisfies all of them. The expectation text is taken from the inner expectation builder,
	///     so it reads like <c>which has name equal to "foo.txt"</c> rather than the raw lambda
	///     source.
	/// </remarks>
	public DidNotTriggerWatcherResult Which(Action<IThat<WatcherChangeDescription>> expectation)
	{
		ThrowHelper.ThrowIfNull(expectation, nameof(expectation));

		ManualExpectationBuilder<WatcherChangeDescription> manualBuilder = new();
		expectation(new ThatSubject<WatcherChangeDescription>(manualBuilder));
		_filter.Add(manualBuilder);
		return this;
	}

	/// <summary>
	///     Allows a <paramref name="timeout" /> for waiting for asynchronous events.
	/// </summary>
	/// <remarks>
	///     Without a timeout, only the changes recorded so far are checked. With it, the expectation waits for the full
	///     timeout before it can succeed, and fails as soon as a matching change is received.
	/// </remarks>
	public DidNotTriggerWatcherResult Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}
}
