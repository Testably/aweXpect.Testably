using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileVersionInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> has the <paramref name="expected" /> product minor part.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> HasProductMinorPart(
		this IThat<IFileVersionInfo> source,
		int expected)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasInt32PropertyConstraint(
					it, grammars, v => v.ProductMinorPart, expected, "product minor part")),
			source);
}
