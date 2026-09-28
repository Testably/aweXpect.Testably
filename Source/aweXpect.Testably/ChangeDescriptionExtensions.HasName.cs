using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect.Testably;

public static partial class ChangeDescriptionExtensions
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" /> name.
	/// </summary>
	/// <remarks>
	///     <see cref="ChangeDescription.Name" /> can be <see langword="null" /> on the underlying
	///     <see cref="ChangeDescription" />, so <paramref name="expected" /> is nullable too.
	/// </remarks>
	public static StringEqualityTypeResult<TChange, IThat<TChange>> HasName<TChange>(
		this IThat<TChange> source,
		string? expected)
		where TChange : ChangeDescription
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<TChange, IThat<TChange>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new NotificationConstraints.HasStringPropertyConstraint<TChange>(
					it, grammars, c => c.Name, options, expected, "name")),
			source,
			options);
	}
}
