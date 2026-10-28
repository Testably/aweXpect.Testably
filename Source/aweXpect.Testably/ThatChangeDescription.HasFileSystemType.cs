using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect;

public static partial class ThatChangeDescription
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" />
	///     <see cref="FileSystemTypes" />.
	/// </summary>
	/// <remarks>
	///     The check uses flag containment, because <see cref="FileSystemTypes" /> is a flag enum.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TChange, IThat<TChange?>> HasFileSystemType<TChange>(
		this IThat<TChange?> subject,
		FileSystemTypes expected)
		where TChange : ChangeDescription
	{
		if (expected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The expected file system type must include at least one flag.", nameof(expected)));
		}

		return new AndOrResult<TChange, IThat<TChange?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars)
				=> new NotificationConstraints.HasFileSystemTypeConstraint<TChange>(it, grammars, expected)),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> does not have the <paramref name="unexpected" />
	///     <see cref="FileSystemTypes" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TChange, IThat<TChange?>> DoesNotHaveFileSystemType<TChange>(
		this IThat<TChange?> subject,
		FileSystemTypes unexpected)
		where TChange : ChangeDescription
	{
		if (unexpected == default)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The unexpected file system type must include at least one flag.", nameof(unexpected)));
		}

		return new AndOrResult<TChange, IThat<TChange?>>(
			subject.Get().ExpectationBuilder.AddConstraint(unexpected, static (unexpected, it, grammars)
				=> new NotificationConstraints.HasFileSystemTypeConstraint<TChange>(it, grammars, unexpected).Invert()),
			subject);
	}
}
