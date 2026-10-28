using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatDirectoryInfo
{
	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> has the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IDirectoryInfo, IThat<IDirectoryInfo?>> HasName(
		this IThat<IDirectoryInfo?> subject,
		string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IDirectoryInfo, IThat<IDirectoryInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new FileSystemConstraints.HasNameConstraint<IDirectoryInfo>(it, grammars, s.options, s.expected)),
			subject,
			options);
	}
}
