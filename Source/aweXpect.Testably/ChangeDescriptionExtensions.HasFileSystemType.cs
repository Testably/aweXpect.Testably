using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" />
	///     <see cref="FileSystemTypes" />.
	/// </summary>
	/// <remarks>
	///     The check uses flag containment, because <see cref="FileSystemTypes" /> is a flag enum.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TChange, IThat<TChange>> HasFileSystemType<TChange>(
		this IThat<TChange> source,
		FileSystemTypes expected)
		where TChange : ChangeDescription
	{
		if (expected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The expected file system type must include at least one flag.", nameof(expected)));
		}

		return new AndOrResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars)
				=> new NotificationConstraints.HasFileSystemTypeConstraint<TChange>(it, grammars, expected)),
			source);
	}

	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> does not have the <paramref name="unexpected" />
	///     <see cref="FileSystemTypes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TChange, IThat<TChange>> DoesNotHaveFileSystemType<TChange>(
		this IThat<TChange> source,
		FileSystemTypes unexpected)
		where TChange : ChangeDescription
	{
		if (unexpected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The unexpected file system type must include at least one flag.", nameof(unexpected)));
		}

		return new AndOrResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint(unexpected, static (unexpected, it, grammars)
				=> new NotificationConstraints.HasFileSystemTypeConstraint<TChange>(it, grammars, unexpected).Invert()),
			source);
	}
}
