using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatFileInfo
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> has the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IFileInfo, IThat<IFileInfo?>> HasName(this IThat<IFileInfo?> subject,
		string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new FileSystemConstraints.HasNameConstraint<IFileInfo>(it, grammars, s.options, s.expected)),
			subject,
			options);
	}
}
