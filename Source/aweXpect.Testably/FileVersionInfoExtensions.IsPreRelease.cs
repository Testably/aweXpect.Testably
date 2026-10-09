using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileVersionInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is a pre-release.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> IsPreRelease(
		this IThat<IFileVersionInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsPreRelease, "is pre-release", "is not pre-release")),
			source);

	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is not a pre-release.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> IsNotPreRelease(
		this IThat<IFileVersionInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsPreRelease, "is pre-release", "is not pre-release").Invert()),
			source);
}
