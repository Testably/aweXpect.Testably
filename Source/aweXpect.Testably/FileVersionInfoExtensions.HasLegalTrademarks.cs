using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileVersionInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> has the <paramref name="expected" /> legal trademarks.
	/// </summary>
	public static StringEqualityTypeResult<IFileVersionInfo, IThat<IFileVersionInfo>> HasLegalTrademarks(
		this IThat<IFileVersionInfo> source,
		string? expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IFileVersionInfo, IThat<IFileVersionInfo>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasStringPropertyConstraint(
					it, grammars, v => v.LegalTrademarks, options, expected, "legal trademarks")),
			source,
			options);
	}
}
