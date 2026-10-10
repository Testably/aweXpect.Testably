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
	///     Checks <see cref="ITimerMock.ExecutionCount" /> once. With <c>.Within(timeout)</c>, it is checked again in the
	///     interval of <c>.CheckEvery(interval)</c> until the quantifier is decided or the timeout expires, so an upper
	///     bound like <c>Never()</c> waits for the full timeout before it can succeed.
	/// </remarks>
	[GuaranteesNotNull]
	public static TimerExecutedResult Executed(
		this IThat<ITimerMock> subject)
	{
		Quantifier quantifier = new();
		RepeatedCheckOptions options = new();
		return new TimerExecutedResult(
			subject.Get().ExpectationBuilder.AddConstraint((quantifier, options), static (s, it, grammars)
				=> new TimerConstraints.TimerExecutedConstraint(it, grammars, s.quantifier, s.options)),
			subject,
			quantifier,
			options);
	}
}
