using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect;

public static partial class ThatChangeDescription
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" /> path.
	/// </summary>
	/// <remarks>
	///     <see cref="ChangeDescription.Path" /> is always set on the underlying
	///     <see cref="ChangeDescription" />, so <paramref name="expected" /> is non-nullable.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TChange, IThat<TChange?>> HasPath<TChange>(
		this IThat<TChange?> subject,
		string expected)
		where TChange : ChangeDescription
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<TChange, IThat<TChange?>>(
			subject.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new NotificationConstraints.HasStringPropertyConstraint<TChange>(
					it, grammars, c => c.Path, s.options, s.expected, "path")),
			subject,
			options);
	}
}
