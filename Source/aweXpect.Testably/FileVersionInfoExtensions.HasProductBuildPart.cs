using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileVersionInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> has the <paramref name="expected" /> product build part.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> HasProductBuildPart(
		this IThat<IFileVersionInfo> source,
		int expected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars)
				=> new FileVersionInfoConstraints.HasInt32PropertyConstraint(
					it, grammars, v => v.ProductBuildPart, expected, "product build part")),
			source);
}
