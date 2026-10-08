using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.TimeSystem;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for <see cref="TimerExtensions.Executed(aweXpect.Core.IThat{ITimerMock})" />.
/// </summary>
public class TimerExecutedResult
	: AndOrResult<ITimerMock, IThat<ITimerMock>, TimerExecutedResult>,
		IOptionsProvider<Quantifier>
{
	private readonly NotificationTimeoutOptions _options;
	private readonly Quantifier _quantifier;

	internal TimerExecutedResult(
		ExpectationBuilder expectationBuilder,
		IThat<ITimerMock> subject,
		Quantifier quantifier,
		NotificationTimeoutOptions options)
		: base(expectationBuilder, subject)
	{
		_quantifier = quantifier;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => _quantifier;

	/// <summary>
	///     Allows a <paramref name="timeout" /> for waiting for asynchronous timer executions.
	/// </summary>
	public TimerExecutedResult Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}
}
