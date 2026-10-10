using System;
using System.IO;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" />
	///     <see cref="NotifyFilters" />.
	/// </summary>
	/// <remarks>
	///     The check uses flag containment, because <see cref="NotifyFilters" /> is a flag enum.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TChange, IThat<TChange>> HasNotifyFilters<TChange>(
		this IThat<TChange> source,
		NotifyFilters expected)
		where TChange : ChangeDescription
	{
		if (expected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The expected notify filters must include at least one flag.", nameof(expected)));
		}

		return new AndOrResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasNotifyFiltersConstraint<TChange>(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> does not have the <paramref name="unexpected" />
	///     <see cref="NotifyFilters" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TChange, IThat<TChange>> DoesNotHaveNotifyFilters<TChange>(
		this IThat<TChange> source,
		NotifyFilters unexpected)
		where TChange : ChangeDescription
	{
		if (unexpected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The unexpected notify filters must include at least one flag.", nameof(unexpected)));
		}

		return new AndOrResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasNotifyFiltersConstraint<TChange>(it, grammars, unexpected).Invert()),
			source);
	}
}
