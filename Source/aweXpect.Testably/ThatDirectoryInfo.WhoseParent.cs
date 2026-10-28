#if NET10_0_OR_GREATER
using System;
using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect;

public static partial class ThatDirectoryInfo
{
	extension(IThat<IDirectoryInfo?> subject)
	{
		/// <summary>
		///     Further expectations on the parent directory of the <see cref="IDirectoryInfo" />.
		/// </summary>
		public IThatSubject<IDirectoryInfo> WhoseParent
			=> new ThatSubject<IDirectoryInfo>(
				subject.Get().ExpectationBuilder
					.ForWhich<IDirectoryInfo, IDirectoryInfo>(GetParentOrThrow, " whose parent ", "the parent"));
	}

	private static IDirectoryInfo GetParentOrThrow(IDirectoryInfo directory)
		=> directory.Parent ?? throw new InvalidOperationException(
			"Cannot assert on the parent of a root directory because it has no parent.");
}
#endif
