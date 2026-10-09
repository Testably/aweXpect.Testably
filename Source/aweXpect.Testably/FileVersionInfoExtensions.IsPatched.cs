using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class FileVersionInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is patched.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> IsPatched(this IThat<IFileVersionInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsPatched, "is patched", "is not patched")),
			source);

	/// <summary>
	///     Verifies that the <see cref="IFileVersionInfo" /> is not patched.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileVersionInfo, IThat<IFileVersionInfo>> IsNotPatched(
		this IThat<IFileVersionInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileVersionInfoConstraints.HasBoolPropertyConstraint(
					it, grammars, v => v.IsPatched, "is patched", "is not patched").Invert()),
			source);
}
