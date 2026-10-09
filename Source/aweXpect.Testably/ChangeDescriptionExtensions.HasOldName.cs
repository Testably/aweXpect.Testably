using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" /> old name.
	/// </summary>
	/// <remarks>
	///     The old name is only set on a <see cref="System.IO.WatcherChangeTypes.Renamed" /> change,
	///     so <see cref="ChangeDescription.OldName" /> is nullable and <paramref name="expected" /> is too.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TChange, IThat<TChange>> HasOldName<TChange>(
		this IThat<TChange> source,
		string? expected)
		where TChange : ChangeDescription
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasStringPropertyConstraint<TChange>(
					it, grammars, c => c.OldName, options, expected, "old name")),
			source,
			options);
	}
}
