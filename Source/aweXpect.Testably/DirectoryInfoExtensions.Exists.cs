using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> exists.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> Exists(this IThat<IDirectoryInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.ExistsConstraint<IDirectoryInfo>(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> does not exist.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo>> DoesNotExist(this IThat<IDirectoryInfo> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.ExistsConstraint<IDirectoryInfo>(it, grammars).Invert()),
			source);
}
