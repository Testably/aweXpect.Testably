using System.IO.Abstractions;
using aweXpect.Core;

namespace aweXpect.Testably.Results;

public partial class DirectoryResult<TType, TThat>
{
	/// <summary>
	///     Further expectations on the <see cref="IDirectoryInfo" /> identified by this directory result.
	/// </summary>
	public IThatSubject<IDirectoryInfo> Which
		=> new ThatSubject<IDirectoryInfo>(
			_expectationBuilder.ForWhich<TType, IDirectoryInfo>(
				ResolveDirectoryInfo, " which ", "the directory"));

	private IDirectoryInfo? ResolveDirectoryInfo(TType source)
	{
		(IFileSystem fs, string fullPath) = _resolver(source);
		return fs.DirectoryInfo.New(fullPath);
	}
}
