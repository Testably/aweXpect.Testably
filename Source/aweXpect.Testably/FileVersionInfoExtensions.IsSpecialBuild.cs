using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileVersionInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is a special build.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> IsSpecialBuild(
		this IThat<IFileVersionInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsSpecialBuild, "is special build", "is not special build")),
			source);

	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is not a special build.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> IsNotSpecialBuild(
		this IThat<IFileVersionInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsSpecialBuild, "is special build", "is not special build").Invert()),
			source);
}
