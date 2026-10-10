using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably.Results;

public partial class FileResult<TParent>
{
	/// <summary>
	///     Further expectations on the <see cref="IFileInfo" /> identified by this file result.
	/// </summary>
	public IThat<IFileInfo> Which
		=> new ThatSubject<IFileInfo>(
			_expectationBuilder.ForWhich<TParent, IFileInfo>(
				ResolveFileInfo, " which ", "the file"));

	private IFileInfo? ResolveFileInfo(TParent source)
	{
		(IFileSystem fs, string fullPath) = _resolver(source);
		return fs.FileInfo.New(fullPath);
	}
}
