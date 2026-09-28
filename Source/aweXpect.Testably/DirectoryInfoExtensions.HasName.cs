using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> has the <paramref name="expected" /> name.
	/// </summary>
	public static StringEqualityTypeResult<IDirectoryInfo, IThat<IDirectoryInfo>> HasName(
		this IThat<IDirectoryInfo> source,
		string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IDirectoryInfo, IThat<IDirectoryInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.HasNameConstraint<IDirectoryInfo>(it, grammars, options, expected)),
			source,
			options);
	}
}
