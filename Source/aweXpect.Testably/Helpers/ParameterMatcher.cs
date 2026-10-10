using System;
using aweXpect.Core;
using Testably.Abstractions.Testing.Statistics;

namespace aweXpect.Testably.Helpers;

internal sealed class ParameterMatcher
{
	private readonly Func<ParameterDescription, bool>? _matcher;

	private ParameterMatcher(string name, Func<ParameterDescription, bool>? matcher, string? expression)
	{
		Name = name;
		Expression = expression;
		_matcher = matcher;
	}

	public string Name { get; }

	public string? Expression { get; }

	public bool IsAny => _matcher is null;

	public static ParameterMatcher From<T>(string name, Func<T, bool>? predicate, string? expression)
	{
		if (predicate is null)
		{
			return new ParameterMatcher(name, null, null);
		}

		string thrower = $"the {name} predicate";
		return new ParameterMatcher(name, p => UserCode.Invoke(p.Is, predicate, thrower), expression);
	}

	public bool IsMatch(ParameterDescription parameter)
		=> _matcher is null || _matcher(parameter);
}
