using System.IO.Abstractions;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;
using aweXpect.Testably.Helpers;

namespace aweXpect;

public static partial class ThatFileInfo
{
	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> exists.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo?>> Exists(this IThat<IFileInfo?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.ExistsConstraint<IFileInfo>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="IFileInfo" /> does not exist.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IFileInfo, IThat<IFileInfo?>> DoesNotExist(this IThat<IFileInfo?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new FileSystemConstraints.ExistsConstraint<IFileInfo>(it, grammars).Invert()),
			subject);
}
