using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Testably.Helpers;
using aweXpect.Testably.Results;
using Testably.Abstractions.Testing.TimeSystem;

namespace aweXpect.Testably;

/// <summary>
///     Extensions for <see cref="ITimerMock" />.
/// </summary>
public static class TimerExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ITimerMock" /> callback was executed.
	/// </summary>
	/// <remarks>
	///     Polls <see cref="ITimerMock.ExecutionCount" /> until either the quantifier is satisfied
	///     or the timeout expires (30 seconds by default; use <c>.Within(timeout)</c> to override).
	/// </remarks>
	[GuaranteesNotNull]
	public static TimerExecutedResult Executed(
		this IThat<ITimerMock> subject)
	{
		Quantifier quantifier = new();
		NotificationTimeoutOptions options = new();
		return new TimerExecutedResult(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new TimerConstraints.TimerExecutedConstraint(it, grammars, quantifier, options)),
			subject,
			quantifier,
			options);
	}
}
