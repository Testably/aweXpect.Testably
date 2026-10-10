using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" /> path.
	/// </summary>
	/// <remarks>
	///     <see cref="ChangeDescription.Path" /> is always set on the underlying
	///     <see cref="ChangeDescription" />, so <paramref name="expected" /> is non-nullable.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TChange, IThat<TChange>> HasPath<TChange>(
		this IThat<TChange> source,
		string expected)
		where TChange : ChangeDescription
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new NotificationConstraints.HasStringPropertyConstraint<TChange>(
					it, grammars, c => c.Path, s.options, s.expected, "path")),
			source,
			options);
	}
}
