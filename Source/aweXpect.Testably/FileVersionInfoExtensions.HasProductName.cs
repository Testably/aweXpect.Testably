using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileVersionInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> has the <paramref name="expected" /> product name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IFileVersionInfo, IThat<IFileVersionInfo>> HasProductName(
		this IThat<IFileVersionInfo> subject,
		string? expected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IFileVersionInfo, IThat<IFileVersionInfo>>(
			subject.Get().ExpectationBuilder.AddConstraint((options, expected), static (s, it, grammars)
				=> new FileVersionInfoConstraints.HasStringPropertyConstraint(
					it, grammars, v => v.ProductName, s.options, s.expected, "product name")),
			subject,
			options);
	}
}
