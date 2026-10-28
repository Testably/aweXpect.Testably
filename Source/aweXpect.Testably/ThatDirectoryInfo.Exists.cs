using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatDirectoryInfo
{
	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> exists.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo?>> Exists(this IThat<IDirectoryInfo?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.ExistsConstraint<IDirectoryInfo>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="IDirectoryInfo" /> does not exist.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IDirectoryInfo, IThat<IDirectoryInfo?>> DoesNotExist(this IThat<IDirectoryInfo?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.ExistsConstraint<IDirectoryInfo>(it, grammars).Invert()),
			subject);
}
