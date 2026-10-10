#if NET10_0_OR_GREATER
using System;
using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably;

public static partial class FileInfoExtensions
{
	extension(IThat<IFileInfo> source)
	{
		/// <summary>
		///     Further expectations on the parent directory of the <see cref="IFileInfo" />.
		/// </summary>
		public IThat<IDirectoryInfo> WhoseParent
			=> new ThatSubject<IDirectoryInfo>(
				source.Get().ExpectationBuilder
					.ForWhich<IFileInfo, IDirectoryInfo>(GetDirectoryOrThrow, " whose parent ", "the parent"));
	}

	private static IDirectoryInfo GetDirectoryOrThrow(IFileInfo file)
		=> file.Directory ?? throw new InvalidOperationException(
			"Cannot assert on the parent directory of the file because it has none.");
}
#endif
