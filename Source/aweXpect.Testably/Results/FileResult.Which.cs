using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably.Results;

public partial class FileResult<TType, TThat>
{
	/// <summary>
	///     Further expectations on the <see cref="IFileInfo" /> identified by this file result.
	/// </summary>
	public IThatSubject<IFileInfo> Which
		=> new ThatSubject<IFileInfo>(
			_expectationBuilder.ForWhich<TType, IFileInfo>(
				ResolveFileInfo, " which ", "the file"));

	private IFileInfo? ResolveFileInfo(TType source)
	{
		(IFileSystem fs, string fullPath) = _resolver(source);
		return fs.FileInfo.New(fullPath);
	}
}
