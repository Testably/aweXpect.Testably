using System;
using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for <see cref="FileSystemWatcherExtensions.Triggered(aweXpect.Core.IThat{IFileSystemWatcher})" />.
/// </summary>
public class TriggeredWatcherResult
	: AndOrResult<IFileSystemWatcher, IThat<IFileSystemWatcher>, TriggeredWatcherResult>,
		IOptionsProvider<Quantifier>
{
	private readonly NotificationConstraints.TriggerNotificationFilter<WatcherChangeDescription> _filter;
	private readonly RepeatedCheckOptions _options;
	private readonly Quantifier _quantifier;

	internal TriggeredWatcherResult(
		ExpectationBuilder expectationBuilder,
		IThat<IFileSystemWatcher> subject,
		Quantifier quantifier,
		RepeatedCheckOptions options, NotificationConstraints.TriggerNotificationFilter<WatcherChangeDescription> filter)
		: base(expectationBuilder, subject)
	{
		_quantifier = quantifier;
		_options = options;
		_filter = filter;
	}

	/// <summary>
	///     Restricts the assertion to events that satisfy the inner <paramref name="expectation" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectation" /> is applied as an additional per-event filter, so any
	///     assertions from <see cref="ChangeDescriptionExtensions" /> (e.g. <c>.HasName(...)</c>,
	///     <c>.HasChangeType(...)</c>) compose naturally (only events that satisfy all of them
	///     count toward the quantifier). The expectation text is taken from the inner expectation
	///     builder, so it reads like <c>which has name equal to "foo.txt"</c> rather than the raw
	///     lambda source.
	/// </remarks>
	public TriggeredWatcherResult Which(Action<IThat<WatcherChangeDescription>> expectation)
	{
		ThrowHelper.ThrowIfNull(expectation, nameof(expectation));

		ManualExpectationBuilder<WatcherChangeDescription> manualBuilder = new();
		expectation(new ThatSubject<WatcherChangeDescription>(manualBuilder));
		_filter.Add(manualBuilder);
		return this;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => _quantifier;

	/// <summary>
	///     Allows a <paramref name="timeout" /> for waiting for asynchronous events.
	/// </summary>
	/// <remarks>
	///     Without a timeout, only the changes recorded so far are checked. With it, the expectation waits until the
	///     quantifier is decided or the timeout elapses, so an upper bound like <c>Never()</c> waits for the full timeout.
	/// </remarks>
	public TriggeredWatcherResult Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}
}
