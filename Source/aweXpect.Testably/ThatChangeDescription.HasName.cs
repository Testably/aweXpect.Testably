using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;
using Testably.Abstractions.Testing.FileSystem;

namespace aweXpect;

public static partial class ThatChangeDescription
{
	/// <summary>
	///     Verifies that the <see cref="ChangeDescription" /> has the <paramref name="expected" /> name.
	/// </summary>
	/// <remarks>
	///     <see cref="ChangeDescription.Name" /> can be <see langword="null" /> on the underlying
	///     <see cref="ChangeDescription" />, so <paramref name="expected" /> is nullable too.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TChange, IThat<TChange?>> HasName<TChange>(
		this IThat<TChange?> subject,
		string? expected)
		where TChange : ChangeDescription
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<TChange, IThat<TChange?>>(
			subject.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new NotificationConstraints.HasStringPropertyConstraint<TChange>(
					it, grammars, c => c.Name, s.options, s.expected, "name")),
			subject,
			options);
	}
}
