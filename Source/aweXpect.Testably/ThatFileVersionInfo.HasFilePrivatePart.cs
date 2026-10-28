using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatFileVersionInfo
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> has the <paramref name="expected" /> file private part.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo?>> HasFilePrivatePart(
		this IThat<IFileVersionInfo?> subject,
		int expected)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(expected, static (expected, it, grammars)
				=> new FileVersionInfoConstraints.HasInt32PropertyConstraint(
					it, grammars, v => v.FilePrivatePart, expected, "file private part")),
			subject);
}
