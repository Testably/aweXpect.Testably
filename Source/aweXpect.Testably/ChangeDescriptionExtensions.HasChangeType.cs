using System;
using System.IO;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

/// <summary>
///     Extensions for <see cref="ChangeDescription" />.
/// </summary>
public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" />
	///     <see cref="WatcherChangeTypes" />.
	/// </summary>
	/// <remarks>
	///     The check uses flag containment, because <see cref="WatcherChangeTypes" /> is a flag enum.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TChange, IThat<TChange>> HasChangeType<TChange>(
		this IThat<TChange> source,
		WatcherChangeTypes expected)
		where TChange : ChangeDescription
	{
		if (expected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The expected change type must include at least one flag.", nameof(expected)));
		}

		return new AndOrResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars)
				=> new NotificationConstraints.HasChangeTypeConstraint<TChange>(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> does not have the <paramref name="unexpected" />
	///     <see cref="WatcherChangeTypes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TChange, IThat<TChange>> DoesNotHaveChangeType<TChange>(
		this IThat<TChange> source,
		WatcherChangeTypes unexpected)
		where TChange : ChangeDescription
	{
		if (unexpected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The unexpected change type must include at least one flag.", nameof(unexpected)));
		}

		return new AndOrResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint(unexpected, static (unexpected, it, grammars)
				=> new NotificationConstraints.HasChangeTypeConstraint<TChange>(it, grammars, unexpected).Invert()),
			source);
	}
}
