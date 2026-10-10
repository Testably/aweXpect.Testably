using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for <see cref="FileSystemExtensions.DidNotTriggerNotification(aweXpect.Core.IThat{MockFileSystem})" />.
/// </summary>
public class DidNotTriggerNotificationResult
	: AndOrResult<MockFileSystem, IThat<MockFileSystem>, DidNotTriggerNotificationResult>
{
	private readonly NotificationConstraints.TriggerNotificationFilter<ChangeDescription> _filter;
	private readonly RepeatedCheckOptions _options;

	internal DidNotTriggerNotificationResult(
		ExpectationBuilder expectationBuilder,
		IThat<MockFileSystem> subject,
		RepeatedCheckOptions options, NotificationConstraints.TriggerNotificationFilter<ChangeDescription> filter)
		: base(expectationBuilder, subject)
	{
		_options = options;
		_filter = filter;
	}

	/// <summary>
	///     Restricts the assertion to notifications that satisfy the inner <paramref name="expectation" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectation" /> is applied as an additional per-change filter, so any
	///     assertions from <see cref="ChangeDescriptionExtensions" /> (e.g. <c>.HasName(...)</c>,
	///     <c>.HasChangeType(...)</c>) compose naturally (the assertion fails if any notification
	///     satisfies all of them). The expectation text is taken from the inner expectation builder,
	///     so it reads like <c>which has name equal to "foo.txt"</c> rather than the raw lambda
	///     source.
	/// </remarks>
	public DidNotTriggerNotificationResult Which(Action<IThat<ChangeDescription>> expectation)
	{
		ThrowHelper.ThrowIfNull(expectation, nameof(expectation));

		ManualExpectationBuilder<ChangeDescription> manualBuilder = new();
		expectation(new ThatSubject<ChangeDescription>(manualBuilder));
		_filter.Add(manualBuilder);
		return this;
	}

	/// <summary>
	///     Allows a <paramref name="timeout" /> for waiting for asynchronous notifications.
	/// </summary>
	/// <remarks>
	///     Without a timeout, only the changes recorded so far are checked. With it, the expectation waits for the full
	///     timeout before it can succeed, and fails as soon as a matching change is received.
	/// </remarks>
	public DidNotTriggerNotificationResult Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}
}
