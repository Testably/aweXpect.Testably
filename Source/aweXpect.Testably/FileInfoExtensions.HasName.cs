using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> has the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>> HasName(this IThat<IFileInfo> source,
		string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));

		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IFileInfo, IThat<IFileInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new FileSystemConstraints.HasNameConstraint<IFileInfo>(it, grammars, s.options, s.expected)),
			source,
			options);
	}
}
