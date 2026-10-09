using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileVersionInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is a debug build.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> IsDebug(this IThat<IFileVersionInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsDebug, "is debug", "is not debug")),
			source);

	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is not a debug build.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> IsNotDebug(this IThat<IFileVersionInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsDebug, "is debug", "is not debug").Invert()),
			source);
}
