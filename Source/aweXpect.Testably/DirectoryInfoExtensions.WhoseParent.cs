#if NET10_0_OR_GREATER
using System;
using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably;

public static partial class DirectoryInfoExtensions
{
	extension(IThat<IDirectoryInfo> source)
	{
		/// <summary>
		///     Further expectations on the parent directory of the <see cref="IDirectoryInfo" />.
		/// </summary>
		public IThat<IDirectoryInfo> WhoseParent
			=> new ThatSubject<IDirectoryInfo>(
				source.Get().ExpectationBuilder
					.ForWhich<IDirectoryInfo, IDirectoryInfo>(GetParentOrThrow, " whose parent ", "the parent"));
	}

	private static IDirectoryInfo GetParentOrThrow(IDirectoryInfo directory)
		=> directory.Parent ?? throw new InvalidOperationException(
			"Cannot assert on the parent of a root directory because it has no parent.");
}
#endif
