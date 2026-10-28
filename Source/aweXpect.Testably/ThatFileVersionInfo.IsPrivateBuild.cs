using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatFileVersionInfo
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is a private build.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo?>> IsPrivateBuild(
		this IThat<IFileVersionInfo?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsPrivateBuild, "is private build", "is not private build")),
			subject);

	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is not a private build.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo?>> IsNotPrivateBuild(
		this IThat<IFileVersionInfo?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsPrivateBuild, "is private build", "is not private build").Invert()),
			subject);
}
