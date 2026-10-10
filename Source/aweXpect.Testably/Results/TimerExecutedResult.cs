using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using Testably.Abstractions.Testing.TimeSystem;

namespace aweXpect.Testably.Results;

/// <summary>
///     The result for <see cref="TimerExtensions.Executed(aweXpect.Core.IThat{ITimerMock})" />.
/// </summary>
public class TimerExecutedResult
	: AndOrResult<ITimerMock, IThat<ITimerMock>, TimerExecutedResult>,
		IOptionsProvider<Quantifier>
{
	private readonly RepeatedCheckOptions _options;
	private readonly Quantifier _quantifier;

	internal TimerExecutedResult(
		ExpectationBuilder expectationBuilder,
		IThat<ITimerMock> subject,
		Quantifier quantifier,
		RepeatedCheckOptions options)
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
	/// <remarks>
	///     Without a timeout, the execution count is checked once, without waiting.
	/// </remarks>
	public TimerExecutedResult Within(TimeSpan timeout)
	{
		_options.Within(timeout);
		return this;
	}

	/// <summary>
	///     Sets the <paramref name="interval" /> in which the execution count is checked while waiting
	///     <see cref="Within(TimeSpan)" /> the timeout.
	/// </summary>
	/// <remarks>
	///     Defaults to <c>Customize.aweXpect.Settings().DefaultCheckInterval</c> if not specified.
	/// </remarks>
	public TimerExecutedResult CheckEvery(TimeSpan interval)
	{
		_options.CheckEvery(interval);
		return this;
	}
}
